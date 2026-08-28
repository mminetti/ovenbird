namespace Core.Shared.Specifications;

public class ConfigurationByTypeNameSpec : Specification<Configuration>
{
    public ConfigurationByTypeNameSpec(string name) =>
        Query
            .Where(configuration => configuration.ConfigurationType.Name == name)
            .Include(configuration => configuration.Company)
            .Include(configuration => configuration.ConfigurationType)
            .Include(configuration => configuration.Connectors)
                .ThenInclude(connector => connector.ConnectorImplementation)
            .Include(configuration => configuration.Connectors)
                .ThenInclude(connector => connector.ConnectorFields)
            .Include(configuration => configuration.Connectors)
                .ThenInclude(connector => connector.ConnectorType);
}
