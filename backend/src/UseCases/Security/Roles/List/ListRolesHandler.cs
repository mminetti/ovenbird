using UseCases.Common;

namespace UseCases.Security.Roles.List;

public class ListRolesHandler(IListRolesQueryService query)
{
    public async Task<Result<ItemPagedResult<RoleDto>>> Handle(ListRolesQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.Pagination.DefaultPageSize,
            ct);

        return Result.Success(result);
    }
}
