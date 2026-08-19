using UseCases.Security.Permissions;

namespace UseCases.Security.Roles;

public record RoleDto(int Id, string Name)
{
    public IReadOnlyList<PermissionDto> Permissions { get; init; } = [];
}
