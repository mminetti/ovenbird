namespace UseCases.Connectors.Update;

public record UpdateConnectorFieldInput(int? Id, string Name, string? Value, bool IsSecret, string Operation);

public record UpdateConnectorCommand(
    int ConnectorId,
    string Name,
    string? Description,
    int ConnectorImplementationId,
    IReadOnlyList<UpdateConnectorFieldInput> Fields);
