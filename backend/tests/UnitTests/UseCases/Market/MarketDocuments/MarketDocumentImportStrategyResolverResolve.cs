using UseCases.Market.MarketDocuments;
using UseCases.Market.MarketDocuments.Import.Strategies;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentImportStrategyResolverResolve
{
    [Fact]
    public void ReturnsExactMarketMatchWhenAvailable()
    {
        var defaultStrategy = Substitute.For<IMarketImportStrategy>();
        defaultStrategy.MarketIdentifier.Returns(BigDataImportStrategy.Identifier);

        var b3Strategy = Substitute.For<IMarketImportStrategy>();
        b3Strategy.MarketIdentifier.Returns("b3");

        var resolver = new MarketImportStrategyResolver([defaultStrategy, b3Strategy]);

        resolver.Resolve("B3").ShouldBe(b3Strategy);
    }

    [Fact]
    public void FallsBackToDefaultWhenNoExactMatch()
    {
        var defaultStrategy = Substitute.For<IMarketImportStrategy>();
        defaultStrategy.MarketIdentifier.Returns(BigDataImportStrategy.Identifier);

        var resolver = new MarketImportStrategyResolver([defaultStrategy]);

        resolver.Resolve("unknown-market").ShouldBe(defaultStrategy);
    }
}
