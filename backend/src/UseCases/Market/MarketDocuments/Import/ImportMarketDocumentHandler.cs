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

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();
        var errors = new List<string>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var documents = await ediHandler.ExecuteImportAsync(configuration, ct);

            foreach (var document in documents)
            {
                if (document.Id == 0)
                {
                    var created = await documentRepository.AddAsync(document, ct);

                    var itemImportResult = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
                        new ImportMarketDocumentItemCommand(configuration.Id, created.Id), ct);

                    if (!itemImportResult.IsSuccess)
                    {
                        errors.AddRange(itemImportResult.Errors.Select(message => $"MarketDocument '{created.Id}': {message}"));
                    }

                    documentIds.Add(created.Id);
                }
                else
                {
                    documentIds.Add(document.Id);
                }
            }
        }

        return errors.Count == 0
            ? Result.Success<IReadOnlyList<long>>(documentIds)
            : Result<IReadOnlyList<long>>.Error(new ErrorList(errors, string.Empty));
    }
}
