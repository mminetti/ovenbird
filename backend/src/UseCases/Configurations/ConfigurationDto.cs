namespace UseCases.Configurations;

public record ConfigurationDto(
    int Id,
    string Name,
    string? Description,
    int ConfigurationTypeId,
    string ConfigurationTypeName,
    int? CompanyId,
    string? CompanyName)
{
    public IReadOnlyList<ConfigurationFieldDto> Fields { get; init; } = [];
    public IReadOnlyList<ConfigurationConnectorDto> Connectors { get; init; } = [];
}

public record ConfigurationFieldDto(int Id, string Name, string? Value);

public record ConfigurationConnectorDto(int Id, string Name);
