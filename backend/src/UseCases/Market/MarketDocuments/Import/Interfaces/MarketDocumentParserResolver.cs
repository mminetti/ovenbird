namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public class MarketDocumentParserResolver(IEnumerable<IMarketDocumentParser> parsers)
{
    public IMarketDocumentParser Resolve(string identifier)
    {
        var match = parsers.FirstOrDefault(p =>
            string.Equals(p.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Market Document Parser couldn't resolve identifier '{identifier}'.");
    }
}
