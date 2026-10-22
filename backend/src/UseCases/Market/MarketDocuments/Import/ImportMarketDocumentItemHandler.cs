using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public record ImportMarketDocumentItemCommand(int ConfigurationId, long MarketDocumentId);

public class ImportMarketDocumentItemHandler(
    IRepository<MarketDocumentItem> itemRepository,
    IReadRepository<MarketDocument> documentRepository,
    IReadRepository<Configuration> configurationRepository,
    EdiImportHandlerResolver ediResolver,
    IMessageBus bus)
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
            try
            {
                item.MarketDocumentId = document.Id;

                var created = await itemRepository.AddAsync(item, ct);

                var buildResult = await bus.InvokeAsync<Result>(
                    new MarketDocumentEntityBuilderCommand(configuration.Id, created.Id), ct);

                if (buildResult.IsSuccess)
                {
                    itemIds.Add(created.Id);
                }
                else
                {
                    errors.AddRange(FlattenErrors(buildResult, item.ReferenceNumber));
                }
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(item.ReferenceNumber,
                    $"MarketDocumentItem '{item.ReferenceNumber}': {ex.Message}"));
            }
        }

        return errors.Count == 0
            ? Result.Success<IReadOnlyList<long>>(itemIds)
            : Result<IReadOnlyList<long>>.Invalid(errors);
    }

    private static IEnumerable<ValidationError> FlattenErrors(Result result, string referenceNumber) =>
        result.ValidationErrors
            .Select(v => new ValidationError(v.Identifier, $"MarketDocumentItem '{referenceNumber}': {v.ErrorMessage}"))
            .Concat(result.Errors.Select(message =>
                new ValidationError(referenceNumber, $"MarketDocumentItem '{referenceNumber}': {message}")));
}
