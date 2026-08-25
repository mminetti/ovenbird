namespace UseCases.Market.MarketDocuments.Import.Strategies;

public class MarketImportStrategyResolver(IEnumerable<IMarketImportStrategy> strategies)
{
    public IMarketImportStrategy Resolve(string identifier)
    {
        var match = strategies.FirstOrDefault(s =>
            string.Equals(s.MarketIdentifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ?? strategies.First(s =>
            string.Equals(s.MarketIdentifier, BigDataImportStrategy.Identifier, StringComparison.OrdinalIgnoreCase));
    }
}
