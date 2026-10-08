using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentImportStrategyResolverResolve
{
    [Fact]
    public void ReturnsExactMarketMatchWhenAvailable()
    {
        var defaultStrategy = Substitute.For<IMarketDocumentProcessor>();

        var b3Strategy = Substitute.For<IMarketDocumentProcessor>();
        b3Strategy.Identifier.Returns("b3");

        var resolver = new MarketDocumentProcessorResolver([defaultStrategy, b3Strategy]);

        resolver.Resolve("B3").ShouldBe(b3Strategy);
    }
}
