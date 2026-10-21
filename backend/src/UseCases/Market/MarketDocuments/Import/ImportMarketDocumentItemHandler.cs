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
        var errors = new List<string>();

        foreach (var item in items)
        {
            item.MarketDocumentId = document.Id;

            var created = await itemRepository.AddAsync(item, ct);

            var buildResult = await bus.InvokeAsync<Result>(new MarketDocumentEntityBuilderCommand(configuration.Id, created.Id), ct);

            if (buildResult.IsSuccess)
            {
                itemIds.Add(created.Id);
            }
            else
            {
                errors.AddRange(FlattenErrors(buildResult).Select(message => $"MarketDocumentItem '{created.Id}': {message}"));
            }
        }

        return errors.Count == 0
            ? Result.Success<IReadOnlyList<long>>(itemIds)
            : Result<IReadOnlyList<long>>.Error(new ErrorList(errors, string.Empty));
    }

    private static IEnumerable<string> FlattenErrors(Result result) =>
        result.Errors.Concat(result.ValidationErrors.Select(v => v.ErrorMessage));
}
