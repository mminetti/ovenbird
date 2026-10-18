using Core.Assets;

namespace Infrastructure.Data.Config.Assets;

public class AssetConfiguration : BaseEntityTypeConfiguration<Asset, long>
{
    public override void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.FileName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Extension)
            .HasMaxLength(20);

        builder.Property(x => x.ContentType)
            .HasMaxLength(100);
    }
}
