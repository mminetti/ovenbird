using Web.Security.Roles;

namespace Web.Security.Users;

public record UserRecord(int Id, string Name, string Email, string ExternalIdentifier, bool IsActive, DateTimeOffset LastModifiedAtUtc, string? LastModifiedBy)
{
    public IReadOnlyList<RoleRecord> Roles { get; init; } = [];
}
