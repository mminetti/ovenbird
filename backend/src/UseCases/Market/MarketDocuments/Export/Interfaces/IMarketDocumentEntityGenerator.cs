using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.Settings;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public interface IMarketDocumentEntityGenerator
{
    Task<MarketDocumentItem> CreateAsync(ServiceStartItem serviceStartItem, Configuration configuration, CancellationToken ct);
}
