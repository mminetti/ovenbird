namespace Core.Shared.Specifications;

public class ConnectorByIdSpec : Specification<Connector>
{
    public ConnectorByIdSpec(int connectorId) =>
        Query
            .Where(connector => connector.Id == connectorId)
            .Include(connector => connector.ConnectorImplementation)
                .ThenInclude(connectorImplementation => connectorImplementation.ConnectorType)
            .Include(connector => connector.ConnectorFields);
}
