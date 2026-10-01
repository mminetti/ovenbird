using Web.Endpoints.Security.Permissions;

namespace Web.Endpoints.Security.Roles;

public record RoleRecord(int Id, string Name, DateTimeOffset LastModifiedAtUtc, string? LastModifiedBy)
{
    public IReadOnlyList<PermissionRecord> Permissions { get; init; } = [];
}
