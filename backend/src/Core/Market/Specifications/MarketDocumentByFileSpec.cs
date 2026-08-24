namespace Core.Market.Specifications;

public class MarketDocumentByFileSpec : Specification<MarketDocument>
{
    public MarketDocumentByFileSpec(string file) =>
        Query.Where(document => document.File == file);
}
