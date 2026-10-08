namespace UseCases.Market.MarketDocuments.Import.Processors.Readers;

public class MarketDocumentTransactionReaderResolver(IEnumerable<IMarketDocumentTransactionReader> readers)
{
    public IMarketDocumentTransactionReader Resolve(string set)
    {
        var match = readers.FirstOrDefault(r =>
            string.Equals(r.Set, set, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Market Document Transaction Reader couldn't resolve TransactionSet '{set}'.");
    }
}
