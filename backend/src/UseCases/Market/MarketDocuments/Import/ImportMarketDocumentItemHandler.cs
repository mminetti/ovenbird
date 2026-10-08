using Core.Market;
using Core.Settings;
using UseCases.Market.MarketDocuments.Import.Processors;
using UseCases.Market.MarketDocuments.Import.Strategies;

namespace UseCases.Market.MarketDocuments.Import;

public class ImportMarketDocumentItemHandler(
    IRepository<MarketDocumentItem> itemRepository,
    IReadRepository<MarketDocument> documentReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    MarketImportStrategyResolver strategyResolver,
    MarketDocumentItemProcessorResolver processorResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentItemCommand command, CancellationToken ct)
    {
        var configuration = await configurationReadRepository.GetByIdAsync(command.ConfigurationId, ct)
            ?? throw new InvalidOperationException($"Configuration '{command.ConfigurationId}' was not found.");

        var document = await documentReadRepository.GetByIdAsync(command.MarketDocumentId, ct)
            ?? throw new InvalidOperationException($"MarketDocument '{command.MarketDocumentId}' was not found.");

        var identifier = configuration.GetRequiredValue(HandlerIdentifier);

        var import = strategyResolver.Resolve(identifier);
        var itemProcessor = processorResolver.Resolve(identifier);

        using var content = await import.OpenDocumentAsync(configuration, document.File, ct);

        var items = await itemProcessor.ProcessAsync(content, ct);

        var itemIds = new List<long>();

        foreach (var item in items)
        {
            item.MarketDocumentId = document.Id;

            var created = await itemRepository.AddAsync(item, ct);

            itemIds.Add(created.Id);
        }

        return Result.Success<IReadOnlyList<long>>(itemIds);
    }
}
