namespace Core.MarketExchange.Specifications;

public class MarketDocumentByIdentifierAndCompanySpec : Specification<MarketDocument>
{
    public MarketDocumentByIdentifierAndCompanySpec(string identifier, int companyId) =>
        Query.Where(document => document.Identifier == identifier && document.CompanyId == companyId);
}
