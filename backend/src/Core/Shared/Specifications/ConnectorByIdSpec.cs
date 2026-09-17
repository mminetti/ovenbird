namespace Core.Shared.Specifications;

public class ConnectorByIdSpec : Specification<Connector>
{
    public ConnectorByIdSpec(int connectorId) =>
        Query
            .Where(connector => connector.Id == connectorId)
            .Include(connector => connector.ConnectorType)
            .Include(connector => connector.ConnectorImplementation)
            .Include(connector => connector.ConnectorFields);
}
