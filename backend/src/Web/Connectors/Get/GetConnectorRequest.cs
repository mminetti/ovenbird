namespace Web.Connectors.Get;

public class GetConnectorRequest
{
    public const string Route = "/connectors/{ConnectorId:int}";
    public static string BuildRoute(int connectorId) => Route.Replace("{ConnectorId:int}", connectorId.ToString());

    public int ConnectorId { get; set; }
}
