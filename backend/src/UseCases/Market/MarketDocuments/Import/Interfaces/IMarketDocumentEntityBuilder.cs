using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IMarketDocumentEntityBuilder
{
    Task CreateAsync(MarketDocumentItem item, CancellationToken ct);
}
