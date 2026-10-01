using System.Linq.Expressions;
using Infrastructure.Data.Queries.Common;
using UseCases.Market.Markets;
using UseCases.Market.Markets.List;

namespace Infrastructure.Data.Queries.Market;

public class ListMarketsQueryService(ReadDbContext db, IQueryPropertyMapper<Core.Market.Market> propertyMapper)
    : PagedQueryServiceBase<Core.Market.Market, MarketDto>(propertyMapper), IListMarketsQueryService
{
    protected override IQueryable<Core.Market.Market> GetQuery() => db.Set<Core.Market.Market>();

    protected override Expression<Func<Core.Market.Market, MarketDto>> GetProjection() =>
        m => new MarketDto(m.Id, m.Name, m.Identifier, m.LastModifiedAtUtc, m.LastModifiedBy);

    protected override string GetDefaultOrderBy() => nameof(Core.Market.Market.Id);
}
