using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentItemConfiguration : BaseEntityTypeConfiguration<MarketDocumentItem, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentItem> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Set)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.SubSet)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Purpose)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.SubPurpose)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.OriginalTrackingNumber)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.ServicePointIdentifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Raw)
            .HasColumnType("nvarchar(max)");

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.MarketDocument)
            .WithMany()
            .HasForeignKey(x => x.MarketDocumentId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.MarketDocumentItemStatus)
            .WithMany()
            .HasForeignKey(x => x.MarketDocumentItemStatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
