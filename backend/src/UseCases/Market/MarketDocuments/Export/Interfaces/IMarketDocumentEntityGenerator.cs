using Core.Market;
using Core.Service;
using Core.Settings;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public interface IMarketDocumentEntityGenerator
{
    Task<MarketDocumentItem> CreateAsync(ServiceStartItem serviceStartItem, Configuration configuration, CancellationToken ct);
}
