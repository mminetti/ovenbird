namespace UseCases.Security.Users.Update;

public record UpdateUserCommand(int UserId, string Name, string Email, bool IsActive);
