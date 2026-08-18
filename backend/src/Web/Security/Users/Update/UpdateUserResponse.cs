namespace Web.Security.Users.Update;

public class UpdateUserResponse(UserRecord user)
{
    public UserRecord User { get; set; } = user;
}
