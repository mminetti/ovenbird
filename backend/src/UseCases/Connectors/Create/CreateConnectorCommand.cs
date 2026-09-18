namespace UseCases.Connectors.Create;

public record CreateConnectorCommand(
    string Name,
    string? Description,
    int ConnectorImplementationId,
    IReadOnlyList<ConnectorFieldInput> Fields);
