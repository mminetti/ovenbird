namespace Core.Shared.Specifications;

public class CompanyByIdSpec : Specification<Company>
{
    public CompanyByIdSpec(int companyId) =>
        Query
            .Where(company => company.Id == companyId)
            .Include(company => company.Market);
}
