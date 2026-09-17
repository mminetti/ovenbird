namespace Web.Security.Users.Me;

public record MeRecord(string Name, string Email)
{
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
