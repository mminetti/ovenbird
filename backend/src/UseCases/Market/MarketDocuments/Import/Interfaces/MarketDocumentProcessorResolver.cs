namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public class MarketDocumentProcessorResolver(IEnumerable<IMarketDocumentProcessor> strategies)
{
    public IMarketDocumentProcessor Resolve(string identifier)
    {
        var match = strategies.FirstOrDefault(s =>
            string.Equals(s.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Market Import Strategy couldn't resolve identifier '{identifier}'.");
    }
}
