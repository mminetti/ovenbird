using UseCases.Market.MarketDocuments.Import.Processors;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentItemProcessorResolverResolve
{
    [Fact]
    public void ReturnsExactMatchWhenAvailable()
    {
        var defaultProcessor = Substitute.For<IMarketDocumentItemProcessor>();

        var bigDataProcessor = Substitute.For<IMarketDocumentItemProcessor>();
        bigDataProcessor.Identifier.Returns("BigData");

        var resolver = new MarketDocumentItemProcessorResolver([defaultProcessor, bigDataProcessor]);

        resolver.Resolve("bigdata").ShouldBe(bigDataProcessor);
    }

    [Fact]
    public void ThrowsWhenIdentifierCannotBeResolved()
    {
        var resolver = new MarketDocumentItemProcessorResolver([]);

        Should.Throw<InvalidOperationException>(() => resolver.Resolve("Unknown"));
    }
}
