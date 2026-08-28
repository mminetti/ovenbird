namespace UseCases.Market.MarketDocuments.Import.Strategies;

public class MarketImportStrategyResolver(IEnumerable<IMarketImportStrategy> strategies)
{
    public IMarketImportStrategy Resolve(string identifier)
    {
        var match = strategies.FirstOrDefault(s =>
            string.Equals(s.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Market Import Strategy couldn't resolve identifier '{identifier}'.");
    }
}
