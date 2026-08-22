namespace Core.Common;

public abstract class AuditableEntityBase<TId> : EntityBase<TId>, IAuditableEntity, IAggregateRoot
    where TId : struct, IEquatable<TId>
{
    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
