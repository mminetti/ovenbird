namespace Core.Shared.Specifications;

public class ConfigurationByIdSpec : Specification<Configuration>
{
    public ConfigurationByIdSpec(int configurationId) =>
        Query
            .Where(configuration => configuration.Id == configurationId)
            .Include(configuration => configuration.Company)
            .Include(configuration => configuration.ConfigurationType)
            .Include(configuration => configuration.ConfigurationFields)
            .Include(configuration => configuration.Connectors);
}
