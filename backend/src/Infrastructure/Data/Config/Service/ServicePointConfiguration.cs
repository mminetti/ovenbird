using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class ServicePointConfiguration : BaseEntityTypeConfiguration<ServicePoint, long>
{
    public override void Configure(EntityTypeBuilder<ServicePoint> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Cycle)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Premise)
            .WithMany()
            .HasForeignKey(x => x.PremiseId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Utility)
            .WithMany()
            .HasForeignKey(x => x.UtilityId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.PremiseType)
            .WithMany()
            .HasForeignKey(x => x.PremiseTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Commodity)
            .WithMany()
            .HasForeignKey(x => x.CommodityId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Market)
            .WithMany()
            .HasForeignKey(x => x.MarketId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
