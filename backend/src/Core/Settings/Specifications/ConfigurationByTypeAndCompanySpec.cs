namespace Core.Settings.Specifications;

public class ConfigurationByTypeAndCompanySpec : Specification<Configuration>
{
    public ConfigurationByTypeAndCompanySpec(int typeId, int companyId) =>
        Query
            .Where(configuration => configuration.ConfigurationTypeId == typeId && configuration.CompanyId == companyId)
            .Include(configuration => configuration.Company)
            .Include(configuration => configuration.ConfigurationType)
            .Include(configuration => configuration.ConfigurationFields)
            .Include(configuration => configuration.Connectors);
}
