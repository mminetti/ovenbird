namespace UseCases.Security.Users.GetOrCreate;

public record CurrentUserInfo(int UserId, string ExternalIdentifier, string Name, string Email, bool IsActive)
{
    public IReadOnlyList<UserPermissionDto> Permissions { get; init; } = [];
}
