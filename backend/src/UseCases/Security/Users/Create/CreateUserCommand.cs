namespace UseCases.Security.Users.Create;

public record CreateUserCommand(
    string Name,
    string Email,
    string ExternalIdentifier,
    IReadOnlyList<int> RoleIds);
