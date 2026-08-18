namespace Web.Security.Users.Create;

public class CreateUserRequest
{
    public const string Route = "/security/users";

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ExternalIdentifier { get; set; } = string.Empty;
}
