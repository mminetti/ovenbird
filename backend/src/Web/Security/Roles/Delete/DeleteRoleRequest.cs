namespace Web.Security.Roles.Delete;

public class DeleteRoleRequest
{
    public const string Route = "/security/roles/{RoleId:int}";
    public static string BuildRoute(int roleId) => Route.Replace("{RoleId:int}", roleId.ToString());

    public int RoleId { get; set; }
}
