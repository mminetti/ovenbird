namespace Web.Security.Users.GetById;

public class GetUserByIdRequest
{
    public const string Route = "/security/users/{UserId:int}";
    public static string BuildRoute(int userId) => Route.Replace("{UserId:int}", userId.ToString());

    public int UserId { get; set; }
}
