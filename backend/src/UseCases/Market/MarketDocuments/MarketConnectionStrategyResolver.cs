namespace UseCases.Market.MarketDocuments;

public class MarketConnectionStrategyResolver(IEnumerable<IMarketConnectionStrategy> strategies)
{
    public IMarketConnectionStrategy Resolve(string identifier)
    {
        var match = strategies.FirstOrDefault(s =>
            string.Equals(s.MarketIdentifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ?? strategies.First(s =>
            string.Equals(s.MarketIdentifier, BigDataConnectionStrategy.Identifier, StringComparison.OrdinalIgnoreCase));
    }
}
