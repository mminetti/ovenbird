namespace Web.Configurations;

public record ConfigurationRecord(
    int Id,
    string Name,
    string? Description,
    int ConfigurationTypeId,
    string ConfigurationTypeName,
    int? CompanyId,
    string? CompanyName)
{
    public IReadOnlyList<ConfigurationFieldRecord> Fields { get; init; } = [];
    public IReadOnlyList<ConfigurationConnectorRecord> Connectors { get; init; } = [];
}

public record ConfigurationFieldRecord(int Id, string Name, string? Value);

public record ConfigurationConnectorRecord(int Id, string Name);
