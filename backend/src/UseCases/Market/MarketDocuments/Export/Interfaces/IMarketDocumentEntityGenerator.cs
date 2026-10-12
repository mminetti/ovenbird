using Core.Market;
using Core.Service;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public interface IMarketDocumentEntityGenerator
{
    Task<MarketDocumentItem> CreateAsync(ServiceStartItem serviceStartItem, CancellationToken ct);
}
