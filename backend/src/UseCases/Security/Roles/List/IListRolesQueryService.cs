using UseCases.Common;

namespace UseCases.Security.Roles.List;

public interface IListRolesQueryService
{
    Task<ItemPagedResult<RoleDto>> ListAsync(int page, int perPage, string? search, string? orderBy, CancellationToken ct);
}
