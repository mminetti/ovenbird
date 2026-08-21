using UseCases.Common;

namespace UseCases.Security.Permissions.List;

public interface IListPermissionsQueryService
{
    Task<ItemPagedResult<PermissionDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
