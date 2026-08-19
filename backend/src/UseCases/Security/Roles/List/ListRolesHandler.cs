namespace UseCases.Security.Roles.List;

public class ListRolesHandler(IListRolesQueryService query)
{
    public async Task<Result<PagedResult<RoleDto>>> Handle(ListRolesQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.DEFAULT_PAGE_SIZE,
            ct);

        return Result.Success(result);
    }
}
