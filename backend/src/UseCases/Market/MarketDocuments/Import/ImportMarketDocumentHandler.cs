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
        var errors = new List<ValidationError>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            IReadOnlyList<MarketDocument> documents;
            try
            {
                var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));
                documents = await ediHandler.ExecuteImportAsync(configuration, ct);
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(configuration.Id.ToString(), ex.Message));
                continue;
            }

            foreach (var document in documents)
            {
                if (document.Id != 0)
                {
                    documentIds.Add(document.Id);
                    continue;
                }

                try
                {
                    var created = await documentRepository.AddAsync(document, ct);

                    var itemImportResult = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
                        new ImportMarketDocumentItemCommand(configuration.Id, created.Id), ct);

                    if (!itemImportResult.IsSuccess)
                    {
                        errors.AddRange(itemImportResult.ValidationErrors.Select(v =>
                            new ValidationError(v.Identifier, $"MarketDocument '{document.Identifier}': {v.ErrorMessage}")));
                    }

                    documentIds.Add(created.Id);
                }
                catch (Exception ex)
                {
                    errors.Add(new ValidationError(document.Identifier, ex.Message));
                }
            }
        }

        return errors.Count == 0
            ? Result.Success<IReadOnlyList<long>>(documentIds)
            : Result<IReadOnlyList<long>>.Invalid(errors);
    }
}
