namespace UseCases.Security.Users.GetOrCreate;

public record GetOrCreateCurrentUserCommand(string ExternalIdentifier, string Name, string Email);
