namespace Web.Security.Modules.Get;

public class GetModuleRequest
{
    public const string Route = "/security/modules/{ModuleId:int}";
    public static string BuildRoute(int moduleId) => Route.Replace("{ModuleId:int}", moduleId.ToString());

    public int ModuleId { get; set; }
}
