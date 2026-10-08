using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentImportStrategyResolverResolve
{
    [Fact]
    public void ReturnsExactMarketMatchWhenAvailable()
    {
        var defaultStrategy = Substitute.For<IMarketDocumentImport>();

        var b3Strategy = Substitute.For<IMarketDocumentImport>();
        b3Strategy.Identifier.Returns("b3");

        var resolver = new MarketDocumentImportResolver([defaultStrategy, b3Strategy]);

        resolver.Resolve("B3").ShouldBe(b3Strategy);
    }
}
