using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Import;

public record ImportMarketDocumentCommand();

public class ImportMarketDocumentHandler(
    IRepository<MarketDocument> documentRepository,
    IReadRepository<Configuration> configurationReadRepository,
    EdiImportHandlerResolver ediResolver,
    IMessageBus bus)
{
    private const string HandlerIdentifier = "handler";

    public async Task<ImportMarketDocumentResult> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var imported = new List<string>();
        var errors = new List<ValidationError>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var importResult = await ediHandler.ExecuteImportAsync(configuration, ct);

            errors.AddRange(importResult.Errors);

            foreach (var document in importResult.Documents)
            {
                var created = await documentRepository.AddAsync(document, ct);

                await bus.PublishAsync(new ImportMarketDocumentItemCommand(configuration.Id, created.Id));

                imported.Add(created.Identifier);
            }
        }

        return new ImportMarketDocumentResult(imported, errors);
    }
}
