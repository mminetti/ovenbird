using UseCases.Market.MarketDocuments.Import.BigData.Readers;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentTransactionReaderResolverResolve
{
    [Fact]
    public void ReturnsExactMatchWhenAvailable()
    {
        var usageReader = Substitute.For<IMarketDocumentTransactionReader>();
        usageReader.Set.Returns("867");

        var invoiceReader = Substitute.For<IMarketDocumentTransactionReader>();
        invoiceReader.Set.Returns("810");

        var resolver = new MarketDocumentTransactionReaderResolver([usageReader, invoiceReader]);

        resolver.Resolve("810").ShouldBe(invoiceReader);
    }

    [Fact]
    public void ThrowsWhenTransactionSetCannotBeResolved()
    {
        var resolver = new MarketDocumentTransactionReaderResolver([]);

        Should.Throw<InvalidOperationException>(() => resolver.Resolve("999"));
    }
}
