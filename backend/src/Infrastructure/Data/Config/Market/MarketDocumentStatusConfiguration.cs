using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentStatusConfiguration : BaseEntityTypeConfiguration<MarketDocumentStatus, int>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentStatus> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
