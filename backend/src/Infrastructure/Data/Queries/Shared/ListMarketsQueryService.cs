using System.Linq.Expressions;
using Core.Market;
using Infrastructure.Data.Queries.Common;
using UseCases.Markets;
using UseCases.Markets.List;

namespace Infrastructure.Data.Queries.Shared;

public class ListMarketsQueryService(ReadDbContext db, IQueryPropertyMapper<Market> propertyMapper)
    : PagedQueryServiceBase<Market, MarketDto>(propertyMapper), IListMarketsQueryService
{
    protected override IQueryable<Market> GetQuery() => db.Set<Market>();

    protected override Expression<Func<Market, MarketDto>> GetProjection() =>
        m => new MarketDto(m.Id, m.Name, m.Identifier, m.LastModifiedAtUtc, m.LastModifiedBy);

    protected override string GetDefaultOrderBy() => nameof(Market.Id);
}
