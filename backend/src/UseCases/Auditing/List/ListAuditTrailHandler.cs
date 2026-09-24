namespace UseCases.Auditing.List;

public class ListAuditTrailHandler(IListAuditTrailQueryService query)
{
    public async Task<Result<ItemPagedResult<AuditTrailDto>>> Handle(ListAuditTrailQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.EntityType,
            request.EntityId,
            request.Page ?? 1,
            request.PerPage ?? Constants.Pagination.DefaultPageSize,
            ct);

        return Result.Success(result);
    }
}
