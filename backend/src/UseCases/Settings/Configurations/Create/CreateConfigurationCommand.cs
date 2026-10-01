namespace UseCases.Settings.Configurations.Create;

public record CreateConfigurationCommand(
    string Name,
    string? Description,
    int ConfigurationTypeId,
    int? CompanyId,
    IReadOnlyList<int> ConnectorIds,
    IReadOnlyList<ConfigurationFieldInput> Fields);
