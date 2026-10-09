using Core.Market;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public class ImportMarketDocumentItemHandler(
    IRepository<MarketDocumentItem> itemRepository,
    IReadRepository<MarketDocument> documentReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    IMarketDocumentTransport transport,
    MarketDocumentParserResolver parserResolver,
    IMessageBus bus)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentItemCommand command, CancellationToken ct)
    {
        var configuration = await configurationReadRepository.FirstOrDefaultAsync(
            new ConfigurationByIdSpec(command.ConfigurationId), ct)
            ?? throw new InvalidOperationException($"Configuration '{command.ConfigurationId}' was not found.");

        var document = await documentReadRepository.GetByIdAsync(command.MarketDocumentId, ct)
            ?? throw new InvalidOperationException($"MarketDocument '{command.MarketDocumentId}' was not found.");

        var parser = parserResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

        using var content = await transport.OpenDocumentAsync(configuration, document.File, ct);

        var items = await parser.ReadTransactionsAsync(content, ct);

        var itemIds = new List<long>();

        foreach (var item in items)
        {
            item.MarketDocumentId = document.Id;

            var created = await itemRepository.AddAsync(item, ct);

            await bus.InvokeAsync(new MarketDocumentEntityBuilderCommand(created.Id), ct);

            itemIds.Add(created.Id);
        }

        return Result.Success<IReadOnlyList<long>>(itemIds);
    }
}
