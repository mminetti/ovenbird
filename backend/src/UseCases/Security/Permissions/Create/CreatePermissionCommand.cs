namespace UseCases.Security.Permissions.Create;

public record CreatePermissionCommand(string Name, int ModuleId, string Description);
