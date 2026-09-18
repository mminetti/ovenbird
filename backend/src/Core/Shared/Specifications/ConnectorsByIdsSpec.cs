namespace Core.Shared.Specifications;

public class ConnectorsByIdsSpec : Specification<Connector>
{
    public ConnectorsByIdsSpec(IReadOnlyList<int> ids) =>
        Query.Where(connector => ids.Contains(connector.Id));
}
