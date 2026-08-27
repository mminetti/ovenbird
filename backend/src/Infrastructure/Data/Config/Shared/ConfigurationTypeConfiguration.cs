using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConfigurationTypeConfiguration : IEntityTypeConfiguration<ConfigurationType>
{
    public void Configure(EntityTypeBuilder<ConfigurationType> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
