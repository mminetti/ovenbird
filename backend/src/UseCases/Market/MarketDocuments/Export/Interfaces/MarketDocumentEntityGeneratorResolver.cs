using Microsoft.Extensions.DependencyInjection;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public class MarketDocumentEntityGeneratorResolver(IServiceProvider serviceProvider)
{
    public IMarketDocumentEntityGenerator Resolve(string key) =>
        serviceProvider.GetKeyedService<IMarketDocumentEntityGenerator>(key) ??
            throw new InvalidOperationException($"Market Document Entity Generator couldn't resolve key '{key}'.");
}
