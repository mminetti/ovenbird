namespace UseCases.Connectors.List;

public class ListConnectorsHandler(IListConnectorsQueryService query)
{
    public async Task<Result<ItemPagedResult<ConnectorDto>>> Handle(ListConnectorsQuery request, CancellationToken ct)
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
