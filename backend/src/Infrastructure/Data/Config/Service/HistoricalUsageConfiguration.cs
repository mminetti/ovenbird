using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class HistoricalUsageConfiguration : BaseEntityTypeConfiguration<HistoricalUsage, long>
{
    public override void Configure(EntityTypeBuilder<HistoricalUsage> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
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
    }
}
