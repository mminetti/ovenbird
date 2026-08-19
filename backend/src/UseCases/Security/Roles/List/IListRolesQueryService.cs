namespace UseCases.Security.Roles.List;

public interface IListRolesQueryService
{
    Task<PagedResult<RoleDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
