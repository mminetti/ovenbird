using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentConfiguration : BaseEntityTypeConfiguration<MarketDocument, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocument> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Name)
            .HasMaxLength(500);
    }
}
