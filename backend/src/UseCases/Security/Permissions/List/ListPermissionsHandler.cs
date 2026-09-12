using UseCases.Common;

namespace UseCases.Security.Permissions.List;

public class ListPermissionsHandler(IListPermissionsQueryService query)
{
    public async Task<Result<ItemPagedResult<PermissionDto>>> Handle(ListPermissionsQuery request, CancellationToken ct)
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
