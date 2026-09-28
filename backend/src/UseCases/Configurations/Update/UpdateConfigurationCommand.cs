namespace UseCases.Configurations.Update;

public record UpdateConfigurationFieldInput(int? Id, string Name, string? Value, string Operation);

public record UpdateConfigurationCommand(
    int ConfigurationId,
    string Name,
    string? Description,
    int ConfigurationTypeId,
    int? CompanyId,
    IReadOnlyList<int> ConnectorIds,
    IReadOnlyList<UpdateConfigurationFieldInput> Fields);
