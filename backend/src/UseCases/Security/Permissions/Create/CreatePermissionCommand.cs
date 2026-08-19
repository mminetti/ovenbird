namespace UseCases.Security.Permissions.Create;

public record CreatePermissionCommand(int ModuleId, string Name, string Description);
