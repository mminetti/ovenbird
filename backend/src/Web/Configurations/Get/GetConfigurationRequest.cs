namespace Web.Configurations.Get;

public class GetConfigurationRequest
{
    public const string Route = "/configurations/{ConfigurationId:int}";
    public static string BuildRoute(int configurationId) => Route.Replace("{ConfigurationId:int}", configurationId.ToString());

    public int ConfigurationId { get; set; }
}
