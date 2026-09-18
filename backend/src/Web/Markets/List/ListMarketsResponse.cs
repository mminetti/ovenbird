using UseCases.Common;

namespace Web.Markets.List;

public record ListMarketsResponse : ItemPagedResult<MarketRecord>
{
    public ListMarketsResponse(IReadOnlyList<MarketRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
