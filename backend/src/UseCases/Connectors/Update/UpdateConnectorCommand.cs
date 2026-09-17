namespace UseCases.Connectors.Update;

public record UpdateConnectorCommand(
    int ConnectorId,
    string Name,
    string? Description,
    int ConnectorTypeId,
    int ConnectorImplementationId,
    IReadOnlyList<ConnectorFieldInput> Fields);
