namespace Web.Configurations.Delete;

public class DeleteConfigurationRequest
{
    public const string Route = "/settings/configurations/{ConfigurationId:int}";
    public static string BuildRoute(int configurationId) => Route.Replace("{ConfigurationId:int}", configurationId.ToString());

    public int ConfigurationId { get; set; }
}
