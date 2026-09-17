namespace Web.Connectors;

public record ConnectorRecord(
    int Id,
    string Name,
    string? Description,
    int ConnectorTypeId,
    string ConnectorTypeName,
    int ConnectorImplementationId,
    string ConnectorImplementationName)
{
    public IReadOnlyList<ConnectorFieldRecord> Fields { get; init; } = [];
}

public record ConnectorFieldRecord(int Id, string Name, string? Value, bool IsSecret);
