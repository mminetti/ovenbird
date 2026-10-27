using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using Microsoft.Extensions.Logging;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public record ImportMarketDocumentItemCommand(int ConfigurationId, long MarketDocumentId);

public class ImportMarketDocumentItemHandler(
    IRepository<MarketDocumentItem> itemRepository,
    IReadRepository<MarketDocument> documentRepository,
    IReadRepository<Configuration> configurationRepository,
    EdiImportHandlerResolver ediResolver,
    IMessageBus bus,
    ILogger<ImportMarketDocumentItemHandler> logger)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentItemCommand command, CancellationToken ct)
    {
        var configuration = await configurationRepository.FirstOrDefaultAsync(
            new ConfigurationByIdSpec(command.ConfigurationId), ct)
            ?? throw new InvalidOperationException($"Configuration '{command.ConfigurationId}' was not found.");

        var document = await documentRepository.FirstOrDefaultAsync(new MarketDocumentByIdSpec(command.MarketDocumentId), ct)
            ?? throw new InvalidOperationException($"MarketDocument '{command.MarketDocumentId}' was not found.");

        var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

        var items = await ediHandler.ExecuteItemImportAsync(configuration, document, ct);

        var itemIds = new List<long>();
        var errors = new List<ValidationError>();

        foreach (var item in items)
        {
            MarketDocumentItem? created = null;

            try
            {
                item.MarketDocumentId = document.Id;

                created = await itemRepository.AddAsync(item, ct);

                var buildResult = await bus.InvokeAsync<Result>(
                    new MarketDocumentEntityBuilderCommand(configuration.Id, created.Id), ct);

                if (buildResult.IsSuccess)
                {
                    itemIds.Add(created.Id);
                }
                else
                {
                    errors.AddRange(FlattenErrors(buildResult, created.Id));
                }
            }
            catch (Exception ex)
            {
                var identifier = created is not null
                    ? EntityReference.Format(nameof(MarketDocumentItem), created.Id)
                    : string.Empty;

                errors.Add(new ValidationError(identifier,
                    $"MarketDocumentItem '{item.ReferenceNumber}': {ex.Message}", string.Empty, ValidationSeverity.Error));
            }
        }

        if (errors.Count > 0)
        {
            logger.LogError(
                "MarketDocumentItem import failed for MarketDocument {MarketDocumentId}. Errors={Errors}",
                document.Id, string.Join(", ", errors.Select(e => e.ErrorMessage)));

            return Result<IReadOnlyList<long>>.Invalid(errors);
        }

        return Result.Success<IReadOnlyList<long>>(itemIds);
    }

    // buildResult.ValidationErrors already carry the right EntityReference/Field/Severity (set by
    // MarketDocumentEntityBuilderHandler) - only its plain NotFound-style Errors need wrapping here.
    private static IEnumerable<ValidationError> FlattenErrors(Result result, long itemId) =>
        result.ValidationErrors
            .Concat(result.Errors.Select(message => new ValidationError(
                EntityReference.Format(nameof(MarketDocumentItem), itemId), message, string.Empty, ValidationSeverity.Error)));
}
