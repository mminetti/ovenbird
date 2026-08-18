namespace UseCases.Security.Users;

public record UserDto(int Id, string Name, string Email, bool IsActive);
