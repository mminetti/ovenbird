using Core.MarketExchange;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IMarketDocumentEntityBuilder
{
    Task<Result> CreateAsync(MarketDocumentItem item, CancellationToken ct);
}
