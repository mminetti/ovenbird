namespace Core.Shared.Specifications;

public class ConfigurationByNameSpec : Specification<Configuration>
{
    public ConfigurationByNameSpec(string name) =>
        Query
            .Where(configuration => configuration.Name == name)
            .Include(configuration => configuration.Company)
            .Include(configuration => configuration.ConfigurationType)
            .Include(configuration => configuration.Integrations)
                .ThenInclude(integration => integration.IntegrationImplementation)
            .Include(configuration => configuration.Integrations)
                .ThenInclude(integration => integration.IntegrationFields);
}
