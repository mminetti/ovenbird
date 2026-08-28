using UseCases.Market.MarketDocuments.Import.Strategies;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentImportStrategyResolverResolve
{
    [Fact]
    public void ReturnsExactMarketMatchWhenAvailable()
    {
        var defaultStrategy = Substitute.For<IMarketImportStrategy>();

        var b3Strategy = Substitute.For<IMarketImportStrategy>();
        b3Strategy.Identifier.Returns("b3");

        var resolver = new MarketImportStrategyResolver([defaultStrategy, b3Strategy]);

        resolver.Resolve("B3").ShouldBe(b3Strategy);
    }
}
