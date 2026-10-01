using System.Linq.Expressions;
using Core.Settings;
using Infrastructure.Data.Queries.Common;
using UseCases.Settings.Connectors;
using UseCases.Settings.Connectors.List;

namespace Infrastructure.Data.Queries.Settings;

public class ListConnectorsQueryService(ReadDbContext db, IQueryPropertyMapper<Connector> propertyMapper)
    : PagedQueryServiceBase<Connector, ConnectorDto>(propertyMapper), IListConnectorsQueryService
{
    protected override IQueryable<Connector> GetQuery() => db.Connector;

    protected override Expression<Func<Connector, ConnectorDto>> GetProjection() =>
        c => new ConnectorDto(
            c.Id,
            c.Name,
            c.Description,
            c.ConnectorImplementation.ConnectorTypeId,
            c.ConnectorImplementation.ConnectorType.Name,
            c.ConnectorImplementationId,
            c.ConnectorImplementation.Name,
            c.LastModifiedAtUtc,
            c.LastModifiedBy);

    protected override string GetDefaultOrderBy() => nameof(Connector.Id);
}
