using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class IntegrationConfiguration : BaseEntityTypeConfiguration<Integration, int>
{
    public override void Configure(EntityTypeBuilder<Integration> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);
    }
}
