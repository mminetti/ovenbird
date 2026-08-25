using UseCases.Market.MarketDocuments;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentImportStrategyResolverResolve
{
    [Fact]
    public void ReturnsExactMarketMatchWhenAvailable()
    {
        var defaultStrategy = Substitute.For<IMarketConnectionStrategy>();
        defaultStrategy.MarketIdentifier.Returns(BigDataConnectionStrategy.Identifier);

        var b3Strategy = Substitute.For<IMarketConnectionStrategy>();
        b3Strategy.MarketIdentifier.Returns("b3");

        var resolver = new MarketConnectionStrategyResolver([defaultStrategy, b3Strategy]);

        resolver.Resolve("B3").ShouldBe(b3Strategy);
    }

    [Fact]
    public void FallsBackToDefaultWhenNoExactMatch()
    {
        var defaultStrategy = Substitute.For<IMarketConnectionStrategy>();
        defaultStrategy.MarketIdentifier.Returns(BigDataConnectionStrategy.Identifier);

        var resolver = new MarketConnectionStrategyResolver([defaultStrategy]);

        resolver.Resolve("unknown-market").ShouldBe(defaultStrategy);
    }
}
