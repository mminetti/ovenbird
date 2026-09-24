namespace Core.Common;

// Owned by AuditTrail; no independent identity. Points at another entity involved in the
// change - either the other side of a many-to-many join row, or the parent aggregate of a
// child-entity change - so that entity's history shows this AuditTrail row too.
public class AuditTrailReference
{
    public string ReferencedEntityType { get; set; } = string.Empty;
    public string ReferencedEntityId { get; set; } = string.Empty;
}
