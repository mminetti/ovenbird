using UseCases.Market.MarketDocuments;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentImportStrategyResolverResolve
{
    [Fact]
    public void ReturnsExactMarketMatchWhenAvailable()
    {
        var defaultStrategy = Substitute.For<IMarketDocumentStrategy>();
        defaultStrategy.MarketIdentifier.Returns(BigDataMarketDocumentStrategy.Identifier);

        var b3Strategy = Substitute.For<IMarketDocumentStrategy>();
        b3Strategy.MarketIdentifier.Returns("b3");

        var resolver = new MarketDocumentStrategyResolver([defaultStrategy, b3Strategy]);

        resolver.Resolve("B3").ShouldBe(b3Strategy);
    }

    [Fact]
    public void FallsBackToDefaultWhenNoExactMatch()
    {
        var defaultStrategy = Substitute.For<IMarketDocumentStrategy>();
        defaultStrategy.MarketIdentifier.Returns(BigDataMarketDocumentStrategy.Identifier);

        var resolver = new MarketDocumentStrategyResolver([defaultStrategy]);

        resolver.Resolve("unknown-market").ShouldBe(defaultStrategy);
    }
}
