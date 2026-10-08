namespace UseCases.Market.MarketDocuments.Import.Processors;

public class MarketDocumentItemProcessorResolver(IEnumerable<IMarketDocumentItemProcessor> processors)
{
    public IMarketDocumentItemProcessor Resolve(string identifier)
    {
        var match = processors.FirstOrDefault(p =>
            string.Equals(p.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Market Document Item Processor couldn't resolve identifier '{identifier}'.");
    }
}
