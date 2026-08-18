using Core.Common;

namespace Infrastructure.Data.Config;

public abstract class BaseEntityTypeConfiguration<TBase, TId> : IEntityTypeConfiguration<TBase>
    where TBase : AuditableEntityBase<TId>
    where TId : struct, IEquatable<TId>
{
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(500);

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(500);
    }
}
