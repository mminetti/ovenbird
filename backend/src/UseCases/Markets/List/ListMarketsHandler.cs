namespace UseCases.Markets.List;

public class ListMarketsHandler(IListMarketsQueryService query)
{
    public async Task<Result<ItemPagedResult<MarketDto>>> Handle(ListMarketsQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.Pagination.DefaultPageSize,
            request.Search,
            request.OrderBy,
            ct);

        return Result.Success(result);
    }
}
