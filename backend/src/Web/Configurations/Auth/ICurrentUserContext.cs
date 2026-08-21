namespace Web.Configurations.Auth;

public interface ICurrentUserContext
{
    string? ExternalIdentifier { get; }
    string Name { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}
