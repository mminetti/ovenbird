namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public class MarketDocumentImportResolver(IEnumerable<IMarketDocumentImport> strategies)
{
    public IMarketDocumentImport Resolve(string identifier)
    {
        var match = strategies.FirstOrDefault(s =>
            string.Equals(s.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Market Import Strategy couldn't resolve identifier '{identifier}'.");
    }
}
