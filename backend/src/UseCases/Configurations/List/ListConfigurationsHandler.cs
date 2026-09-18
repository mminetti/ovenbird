namespace UseCases.Configurations.List;

public class ListConfigurationsHandler(IListConfigurationsQueryService query)
{
    public async Task<Result<ItemPagedResult<ConfigurationDto>>> Handle(ListConfigurationsQuery request, CancellationToken ct)
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
