using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentDirectionConfiguration : BaseEntityTypeConfiguration<MarketDocumentDirection, int>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentDirection> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
