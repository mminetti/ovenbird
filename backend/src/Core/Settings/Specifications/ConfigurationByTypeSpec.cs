namespace Core.Settings.Specifications;

public class ConfigurationByTypeSpec : Specification<Configuration>
{
    public ConfigurationByTypeSpec(int typeId) =>
        Query
            .Where(x => x.ConfigurationTypeId == typeId)
            .Include(x => x.Company)
            .Include(x => x.ConfigurationType)
            .Include(x => x.ConfigurationFields)
            .Include(x => x.Connectors)
                .ThenInclude(x => x.ConnectorImplementation)
            .Include(x => x.Connectors)
                .ThenInclude(x => x.ConnectorFields);
}
