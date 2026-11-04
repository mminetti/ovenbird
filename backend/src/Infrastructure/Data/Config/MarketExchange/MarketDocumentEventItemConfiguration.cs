using Core.MarketExchange;
using Infrastructure.Data.Config.Common;

namespace Infrastructure.Data.Config.MarketExchange;

public class MarketDocumentEventItemConfiguration : BaseEntityTypeConfiguration<MarketDocumentEventItem, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentEventItem> builder)
    {
        base.Configure(builder);

        builder.ConfigureErrorLogItem();

        // Cascade: items are owned by the event, delete with it.
        builder.HasOne(x => x.MarketDocumentEvent)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.MarketDocumentEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
