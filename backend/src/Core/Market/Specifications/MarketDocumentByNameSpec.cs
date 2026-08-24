namespace Core.Market.Specifications;

public class MarketDocumentByNameSpec : Specification<MarketDocument>
{
    public MarketDocumentByNameSpec(string name) =>
        Query.Where(document => document.Name == name);
}
