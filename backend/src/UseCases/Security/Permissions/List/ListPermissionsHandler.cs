using UseCases.Common;

namespace UseCases.Security.Permissions.List;

public class ListPermissionsHandler(IListPermissionsQueryService query)
{
    public async Task<Result<ItemPagedResult<PermissionDto>>> Handle(ListPermissionsQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.DEFAULT_PAGE_SIZE,
            ct);

        return Result.Success(result);
    }
}
