namespace UseCases.Security.Users;

public record UserPermissionDto(string Name);

public record CurrentUserProfileDto(string Name, string Email)
{
    public IReadOnlyList<UserPermissionDto> Permissions { get; init; } = [];
}
