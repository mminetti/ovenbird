using UseCases.Market.Markets;

namespace UseCases.Market.Markets.List;

public interface IListMarketsQueryService
{
    Task<ItemPagedResult<MarketDto>> ListAsync(int page, int perPage, string? search, string? orderBy, CancellationToken ct);
}
