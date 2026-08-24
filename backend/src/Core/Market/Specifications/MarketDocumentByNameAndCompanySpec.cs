namespace Core.Market.Specifications;

public class MarketDocumentByNameAndCompanySpec : Specification<MarketDocument>
{
    public MarketDocumentByNameAndCompanySpec(string name, int companyId) =>
        Query.Where(document => document.Name == name && document.CompanyId == companyId);
}
