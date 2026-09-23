using Web.Security.Permissions;

namespace Web.Security.Roles;

public record RoleRecord(int Id, string Name, DateTimeOffset LastModifiedAtUtc, string? LastModifiedBy)
{
    public IReadOnlyList<PermissionRecord> Permissions { get; init; } = [];
}
