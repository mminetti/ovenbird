namespace Core.Shared.Specifications;

public class CompanyWithMarketSpec : Specification<Company>
{
    public CompanyWithMarketSpec() =>
        Query.Include(company => company.Market);
}
