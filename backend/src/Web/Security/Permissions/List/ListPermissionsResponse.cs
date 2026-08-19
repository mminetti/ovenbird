using UseCases;

namespace Web.Security.Permissions.List;

public record ListPermissionsResponse : PagedResult<PermissionRecord>
{
    public ListPermissionsResponse(IReadOnlyList<PermissionRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
