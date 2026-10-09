using Microsoft.Extensions.DependencyInjection;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public class MarketDocumentEntityBuilderResolver(IServiceProvider serviceProvider)
{
    public IMarketDocumentEntityBuilder? Resolve(string key) =>
        serviceProvider.GetKeyedService<IMarketDocumentEntityBuilder>(key);
}
