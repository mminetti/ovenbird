using Core.Common;

namespace Infrastructure.Data.Config.Common;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.ToTable("AuditTrail");

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.UserId)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.AffectedColumns)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new { x.EntityType, x.EntityId });

        // NoAction: references are informational lookups, never a constraint on the
        // referenced row - there's no FK to another table to begin with.
        builder.OwnsMany(x => x.References, r =>
        {
            r.ToTable("AuditTrailReference");
            r.WithOwner().HasForeignKey("AuditTrailId");
            r.Property<int>("Id");
            r.HasKey("Id");

            r.Property(x => x.ReferencedEntityType)
                .IsRequired()
                .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

            r.Property(x => x.ReferencedEntityId)
                .IsRequired()
                .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

            r.HasIndex(x => new { x.ReferencedEntityType, x.ReferencedEntityId });
        });
    }
}
