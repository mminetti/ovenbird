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
                errors.Add(new ValidationError(
                    EntityReference.Format(nameof(Configuration), configuration.Id), ex.Message, string.Empty, ValidationSeverity.Error));
                continue;
            }

            foreach (var document in documents)
            {
                if (document.Id != 0)
                {
                    documentIds.Add(document.Id);
                    continue;
                }

                MarketDocument? created = null;

                try
                {
                    created = await documentRepository.AddAsync(document, ct);

                    await bus.PublishAsync(new ImportMarketDocumentItemCommand(configuration.Id, created.Id));

                    documentIds.Add(created.Id);
                }
                catch (Exception ex)
                {
                    var identifier = created is not null
                        ? EntityReference.Format(nameof(MarketDocument), created.Id)
                        : string.Empty;

                    errors.Add(new ValidationError(identifier,
                        $"MarketDocument '{document.Identifier}': {ex.Message}", string.Empty, ValidationSeverity.Error));
                }
            }
        }

        return errors.Count == 0
            ? Result.Success<IReadOnlyList<long>>(documentIds)
            : Result<IReadOnlyList<long>>.Invalid(errors);
    }
}
