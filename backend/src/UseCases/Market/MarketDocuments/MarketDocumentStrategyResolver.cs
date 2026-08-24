namespace UseCases.Market.MarketDocuments;

public class MarketDocumentStrategyResolver(IEnumerable<IMarketDocumentStrategy> strategies)
{
    public IMarketDocumentStrategy Resolve(string identifier)
    {
        var match = strategies.FirstOrDefault(s =>
            string.Equals(s.MarketIdentifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ?? strategies.First(s =>
            string.Equals(s.MarketIdentifier, BigDataMarketDocumentStrategy.Identifier, StringComparison.OrdinalIgnoreCase));
    }
}
