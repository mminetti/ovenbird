namespace UseCases.Security.Users.SetRoles;

public record SetUserRolesCommand(int UserId, IReadOnlyList<int> RoleIds);
