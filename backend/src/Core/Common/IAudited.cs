namespace Core.Common;

// Marker interface opting an entity into the audit trail (see AuditTrailInterceptor).
// Independent of IAuditableEntity/AuditableEntityBase (CreatedBy/LastModifiedBy stamps) -
// an entity can implement either, both, or neither.
public interface IAudited
{
}
