using Core.Market;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public class MarketDocumentEntityBuilderHandler(
    IReadRepository<MarketDocumentItem> itemReadRepository,
    MarketDocumentEntityBuilderResolver entityBuilderResolver)
{
    public async Task Handle(MarketDocumentEntityBuilderCommand command, CancellationToken ct)
    {
        var item = await itemReadRepository.GetByIdAsync(command.MarketDocumentItemId, ct)
            ?? throw new InvalidOperationException($"MarketDocumentItem '{command.MarketDocumentItemId}' was not found.");

        var entityBuilder = entityBuilderResolver.Resolve($"{item.Set}_{item.SubSet}");

        if (entityBuilder is not null)
        {
            await entityBuilder.CreateAsync(item, ct);
        }
    }
}
