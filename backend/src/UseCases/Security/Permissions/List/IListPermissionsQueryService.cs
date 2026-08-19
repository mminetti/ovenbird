namespace UseCases.Security.Permissions.List;

public interface IListPermissionsQueryService
{
    Task<PagedResult<PermissionDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
