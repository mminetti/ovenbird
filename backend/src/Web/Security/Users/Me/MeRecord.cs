namespace Web.Security.Users.Me;

public record MePermissionRecord(string Name);

public record MeRecord(string Name, string Email)
{
    public IReadOnlyList<MePermissionRecord> Permissions { get; init; } = [];
}
