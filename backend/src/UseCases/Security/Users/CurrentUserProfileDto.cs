namespace UseCases.Security.Users;

public record CurrentUserProfileDto(string Name, string Email)
{
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
