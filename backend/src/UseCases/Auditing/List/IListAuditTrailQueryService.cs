namespace UseCases.Auditing.List;

public interface IListAuditTrailQueryService
{
    Task<ItemPagedResult<AuditTrailDto>> ListAsync(string entityType, string entityId, int page, int perPage, CancellationToken ct);
}
