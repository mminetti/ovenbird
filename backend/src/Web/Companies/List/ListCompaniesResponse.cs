using UseCases.Common;

namespace Web.Companies.List;

public record ListCompaniesResponse : ItemPagedResult<CompanyRecord>
{
    public ListCompaniesResponse(IReadOnlyList<CompanyRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
