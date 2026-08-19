using UseCases.Security.Roles;

namespace UseCases.Security.Users;

public record UserDto(int Id, string Name, string Email, bool IsActive)
{
    public IReadOnlyList<RoleDto> Roles { get; init; } = [];
}
