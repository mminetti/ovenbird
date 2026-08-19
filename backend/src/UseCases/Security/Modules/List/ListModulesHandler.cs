namespace UseCases.Security.Modules.List;

public class ListModulesHandler(IListModulesQueryService query)
{
    public async Task<Result<PagedResult<ModuleDto>>> Handle(ListModulesQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.DEFAULT_PAGE_SIZE,
            ct);

        return Result.Success(result);
    }
}
