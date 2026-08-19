namespace UseCases.Security.Permissions.Update;

public record UpdatePermissionCommand(int PermissionId, int ModuleId, string Name, string Description);
