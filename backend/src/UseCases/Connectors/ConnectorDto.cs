namespace UseCases.Connectors;

public record ConnectorDto(
    int Id,
    string Name,
    string? Description,
    int ConnectorTypeId,
    string ConnectorTypeName,
    int ConnectorImplementationId,
    string ConnectorImplementationName)
{
    public IReadOnlyList<ConnectorFieldDto> Fields { get; init; } = [];
}

public record ConnectorFieldDto(int Id, string Name, string? Value, bool IsSecret);
