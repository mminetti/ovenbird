using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public record MarketDocumentImportCommand();

public class MarketDocumentImportHandler(
    IRepository<MarketDocument> documentRepository,
    IReadRepository<Configuration> configurationReadRepository,
    EdiImportHandlerResolver ediResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result> Handle(MarketDocumentImportCommand command, CancellationToken ct)
    {
        var errors = new List<ValidationError>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(Constants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            var ediImportHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var importResult = await ediImportHandler.ExecuteImportAsync(configuration, ct);

            errors.AddRange(importResult.Errors);

            foreach (var document in importResult.Documents)
            {
                document.MarkImported();

                await documentRepository.AddAsync(document, ct);
            }
        }

        return errors.Count == 0 ? Result.Success() : Result.Invalid(errors);
    }
}
