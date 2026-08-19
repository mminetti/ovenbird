namespace Web.Security.Permissions.Delete;

public class DeletePermissionRequest
{
    public const string Route = "/security/permissions/{PermissionId:int}";
    public static string BuildRoute(int permissionId) => Route.Replace("{PermissionId:int}", permissionId.ToString());

    public int PermissionId { get; set; }
}
