using Core.Common;
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
            new MarketDocumentDirection { Id = Constants.MarketDocumentDirections.Inbound, Name = nameof(Constants.MarketDocumentDirections.Inbound)},
            new MarketDocumentDirection { Id = Constants.MarketDocumentDirections.Outbound, Name = nameof(Constants.MarketDocumentDirections.Outbound)});
    }
}
