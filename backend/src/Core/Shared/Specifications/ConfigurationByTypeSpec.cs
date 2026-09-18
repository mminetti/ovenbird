namespace Core.Shared.Specifications;

public class ConfigurationByTypeSpec : Specification<Configuration>
{
    public ConfigurationByTypeSpec(int typeId) =>
        Query
            .Where(configuration => configuration.ConfigurationTypeId == typeId)
            .Include(configuration => configuration.Company)
            .Include(configuration => configuration.ConfigurationType)
            .Include(configuration => configuration.Connectors)
                .ThenInclude(connector => connector.ConnectorImplementation)
            .Include(configuration => configuration.Connectors)
                .ThenInclude(connector => connector.ConnectorFields);
}
