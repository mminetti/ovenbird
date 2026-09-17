namespace UseCases.Connectors.Create;

public record CreateConnectorCommand(
    string Name,
    string? Description,
    int ConnectorTypeId,
    int ConnectorImplementationId,
    IReadOnlyList<ConnectorFieldInput> Fields);
