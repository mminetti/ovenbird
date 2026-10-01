using System.Linq.Expressions;
using Core.Settings;
using Infrastructure.Data.Queries.Common;
using UseCases.Settings.Configurations;
using UseCases.Settings.Configurations.List;

namespace Infrastructure.Data.Queries.Settings;

public class ListConfigurationsQueryService(ReadDbContext db, IQueryPropertyMapper<Configuration> propertyMapper)
    : PagedQueryServiceBase<Configuration, ConfigurationDto>(propertyMapper), IListConfigurationsQueryService
{
    protected override IQueryable<Configuration> GetQuery() => db.Configuration;

    protected override Expression<Func<Configuration, ConfigurationDto>> GetProjection() =>
        c => new ConfigurationDto(
            c.Id,
            c.Name,
            c.Description,
            c.ConfigurationTypeId,
            c.ConfigurationType.Name,
            c.CompanyId,
            c.Company != null ? c.Company.Name : null,
            c.LastModifiedAtUtc,
            c.LastModifiedBy);

    protected override string GetDefaultOrderBy() => nameof(Configuration.Id);
}
