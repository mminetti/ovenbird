namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public class MarketDocumentGeneratorResolver(IEnumerable<IMarketDocumentGenerator> generators)
{
    public IMarketDocumentGenerator Resolve(string identifier)
    {
        var match = generators.FirstOrDefault(g =>
            string.Equals(g.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Market Document Generator couldn't resolve identifier '{identifier}'.");
    }
}
