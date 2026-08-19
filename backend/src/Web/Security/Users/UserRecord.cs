using Web.Security.Roles;

namespace Web.Security.Users;

public record UserRecord(int Id, string Name, string Email, bool IsActive)
{
    public IReadOnlyList<RoleRecord> Roles { get; init; } = [];
}
