using Core.MarketExchange;

namespace Infrastructure.Data.Config.MarketExchange;

public class MarketDocumentEventConfiguration : BaseEntityTypeConfiguration<MarketDocumentEvent, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentEvent> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Message)
            .HasColumnType("nvarchar(max)");

        // Cascade: events are owned by the item, delete with it.
        builder.HasOne(x => x.MarketDocumentItem)
            .WithMany()
            .HasForeignKey(x => x.MarketDocumentItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.EventType)
            .WithMany()
            .HasForeignKey(x => x.EventTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced lookup row from accidental/cascading deletion.
        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
