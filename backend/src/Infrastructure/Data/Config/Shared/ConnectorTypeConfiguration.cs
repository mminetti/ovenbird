using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConnectorTypeConfiguration : IEntityTypeConfiguration<ConnectorType>
{
    public void Configure(EntityTypeBuilder<ConnectorType> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);
    }
}
