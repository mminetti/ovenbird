namespace UseCases.Security.Roles.SetPermissions;

public record SetRolePermissionsCommand(int RoleId, IReadOnlyList<int> PermissionIds);
