using System.Linq.Expressions;
using Infrastructure.Data.Queries.Common;
using UseCases.Market.Markets;
using UseCases.Market.Markets.List;

namespace Infrastructure.Data.Queries.Markets;

public class ListMarketsQueryService(ReadDbContext db, IQueryPropertyMapper<Core.ServiceDelivery.Market> propertyMapper)
    : PagedQueryServiceBase<Core.ServiceDelivery.Market, MarketDto>(propertyMapper), IListMarketsQueryService
{
    protected override IQueryable<Core.ServiceDelivery.Market> GetQuery() => db.Set<Core.ServiceDelivery.Market>();

    protected override Expression<Func<Core.ServiceDelivery.Market, MarketDto>> GetProjection() =>
        m => new MarketDto(m.Id, m.Name, m.Identifier, m.LastModifiedAtUtc, m.LastModifiedBy);

    protected override string GetDefaultOrderBy() => nameof(Core.ServiceDelivery.Market.Id);
}
