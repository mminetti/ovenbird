using Microsoft.Extensions.DependencyInjection;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public class ServicePointUpdateBuilderResolver(IServiceProvider serviceProvider)
{
    public IServicePointUpdateBuilder? Resolve(string key) =>
        serviceProvider.GetKeyedService<IServicePointUpdateBuilder>(key);
}
