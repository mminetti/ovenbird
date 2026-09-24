namespace UseCases.Security.Permissions;

public record PermissionDto(int Id, string Name, int ModuleId, string ModuleName, string Description);
