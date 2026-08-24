using Core.Constants;
using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentDirectionConfiguration : IEntityTypeConfiguration<MarketDocumentDirection>
{
    public void Configure(EntityTypeBuilder<MarketDocumentDirection> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new MarketDocumentDirection { Id = MarketDocumentDirections.Inbound, Name = nameof(MarketDocumentDirections.Inbound)},
            new MarketDocumentDirection { Id = MarketDocumentDirections.Outbound, Name = nameof(MarketDocumentDirections.Outbound)});
    }
}
