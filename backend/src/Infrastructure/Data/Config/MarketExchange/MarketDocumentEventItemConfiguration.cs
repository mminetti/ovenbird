using Core.MarketExchange;

namespace Infrastructure.Data.Config.MarketExchange;

public class MarketDocumentEventItemConfiguration : BaseEntityTypeConfiguration<MarketDocumentEventItem, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentEventItem> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Field)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Message)
            .HasColumnType("nvarchar(max)");

        // Cascade: items are owned by the event, delete with it.
        builder.HasOne(x => x.MarketDocumentEvent)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.MarketDocumentEventId)
            .OnDelete(DeleteBehavior.Cascade);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Level)
            .WithMany()
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
