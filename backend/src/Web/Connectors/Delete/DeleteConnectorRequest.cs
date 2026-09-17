namespace Web.Connectors.Delete;

public class DeleteConnectorRequest
{
    public const string Route = "/connectors/{ConnectorId:int}";
    public static string BuildRoute(int connectorId) => Route.Replace("{ConnectorId:int}", connectorId.ToString());

    public int ConnectorId { get; set; }
}
