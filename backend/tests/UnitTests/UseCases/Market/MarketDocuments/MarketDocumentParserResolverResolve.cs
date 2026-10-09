using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentParserResolverResolve
{
    [Fact]
    public void ReturnsExactMarketMatchWhenAvailable()
    {
        var defaultParser = Substitute.For<IMarketDocumentParser>();

        var b3Parser = Substitute.For<IMarketDocumentParser>();
        b3Parser.Identifier.Returns("b3");

        var resolver = new MarketDocumentParserResolver([defaultParser, b3Parser]);

        resolver.Resolve("B3").ShouldBe(b3Parser);
    }
}
