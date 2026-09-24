using UseCases.Common;

namespace Web.Auditing.List;

public record ListAuditTrailResponse : ItemPagedResult<AuditTrailRecord>
{
    public ListAuditTrailResponse(IReadOnlyList<AuditTrailRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
