using Web.Security.Permissions;

namespace Web.Security.Roles;

public record RoleRecord(int Id, string Name)
{
    public IReadOnlyList<PermissionRecord> Permissions { get; init; } = [];
}
