using UseCases.Common;

namespace Web.Security.Roles.List;

public record ListRolesResponse : ItemPagedResult<RoleRecord>
{
    public ListRolesResponse(IReadOnlyList<RoleRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
