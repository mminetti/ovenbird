namespace UseCases.Security.Roles.Update;

public record UpdateRoleCommand(int RoleId, string Name, IList<RolePermissionOperationDto>? Permissions);
