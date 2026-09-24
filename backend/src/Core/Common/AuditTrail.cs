namespace Core.Common;

public class AuditTrail : EntityBase<long>, IAggregateRoot
{
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string? UserId { get; set; }
    public DateTimeOffset TimestampUtc { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }

    public ICollection<AuditTrailReference> References { get; set; } = [];
}
