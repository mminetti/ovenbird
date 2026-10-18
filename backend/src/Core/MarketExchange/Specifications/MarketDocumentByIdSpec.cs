namespace Core.MarketExchange.Specifications;

public class MarketDocumentByIdSpec : Specification<MarketDocument>
{
    public MarketDocumentByIdSpec(long id) =>
        Query
            .Where(document => document.Id == id)
            .Include(document => document.Asset);
}
