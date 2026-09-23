using UseCases.Security.Permissions;

namespace UseCases.Security.Roles;

public record RoleDto(int Id, string Name, DateTimeOffset LastModifiedAtUtc, string? LastModifiedBy)
{
    public IReadOnlyList<PermissionDto> Permissions { get; init; } = [];
}
