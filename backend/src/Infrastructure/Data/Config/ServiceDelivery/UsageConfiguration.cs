using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class UsageConfiguration : BaseEntityTypeConfiguration<Usage, long>
{
    public override void Configure(EntityTypeBuilder<Usage> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Service)
            .WithMany(x => x.Usages)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServicePoint)
            .WithMany()
            .HasForeignKey(x => x.ServicePointId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Meter)
            .WithMany()
            .HasForeignKey(x => x.MeterId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Commodity)
            .WithMany()
            .HasForeignKey(x => x.CommodityId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.UsagePurpose)
            .WithMany()
            .HasForeignKey(x => x.UsagePurposeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.UsageType)
            .WithMany()
            .HasForeignKey(x => x.UsageTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.UsageStatus)
            .WithMany()
            .HasForeignKey(x => x.UsageStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: avoid cascading deletes through the self-referencing OriginalUsage link.
        builder.HasOne(x => x.OriginalUsage)
            .WithMany()
            .HasForeignKey(x => x.OriginalUsageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
