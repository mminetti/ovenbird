using UseCases.Common;

namespace Web.Security.Permissions.List;

public record ListPermissionsResponse : ItemPagedResult<PermissionRecord>
{
    public ListPermissionsResponse(IReadOnlyList<PermissionRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
