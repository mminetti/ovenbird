namespace UseCases.Configurations.Update;

public record UpdateConfigurationCommand(
    int ConfigurationId,
    string Name,
    string? Description,
    int ConfigurationTypeId,
    int? CompanyId,
    IReadOnlyList<int> ConnectorIds,
    IReadOnlyList<ConfigurationFieldInput> Fields);
