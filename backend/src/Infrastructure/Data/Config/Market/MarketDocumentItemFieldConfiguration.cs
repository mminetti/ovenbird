using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentItemFieldConfiguration : BaseEntityTypeConfiguration<MarketDocumentItemField, long>
{
    public override void Configure(EntityTypeBuilder<MarketDocumentItemField> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.FieldName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.FieldValue)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);
    }
}
