namespace UseCases.Security.Permissions.Update;

public record UpdatePermissionCommand(int PermissionId, string Name, int ModuleId, string Description);
