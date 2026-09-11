namespace UseCases.Security.Permissions.Update;

public record UpdatePermissionCommand(int PermissionId, string Name, string Description);
