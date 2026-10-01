namespace Core.Settings.Specifications;

public class CompanyWithMarketSpec : Specification<Company>
{
    public CompanyWithMarketSpec() =>
        Query
            .Include(company => company.Market);
}
