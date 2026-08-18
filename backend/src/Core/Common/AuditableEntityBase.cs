namespace Core.Common;

public abstract class AuditableEntityBase<TId> : EntityBase<TId>, IAuditableEntity, IAggregateRoot
    where TId : struct, IEquatable<TId>
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
