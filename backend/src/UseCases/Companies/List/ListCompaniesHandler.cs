namespace UseCases.Companies.List;

public class ListCompaniesHandler(IListCompaniesQueryService query)
{
    public async Task<Result<ItemPagedResult<CompanyDto>>> Handle(ListCompaniesQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.Pagination.DefaultPageSize,
            request.Search,
            request.OrderBy,
            ct);

        return Result.Success(result);
    }
}
