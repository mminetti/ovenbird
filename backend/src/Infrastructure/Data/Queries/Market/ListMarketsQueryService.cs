using System.Linq.Expressions;
using Infrastructure.Data.Queries.Common;
using UseCases.Market.Markets;
using UseCases.Market.Markets.List;

namespace Infrastructure.Data.Queries.Market;

public class ListMarketsQueryService(ReadDbContext db, IQueryPropertyMapper<Core.Service.Market> propertyMapper)
    : PagedQueryServiceBase<Core.Service.Market, MarketDto>(propertyMapper), IListMarketsQueryService
{
    protected override IQueryable<Core.Service.Market> GetQuery() => db.Set<Core.Service.Market>();

    protected override Expression<Func<Core.Service.Market, MarketDto>> GetProjection() =>
        m => new MarketDto(m.Id, m.Name, m.Identifier, m.LastModifiedAtUtc, m.LastModifiedBy);

    protected override string GetDefaultOrderBy() => nameof(Core.Service.Market.Id);
}
