using Core.Common;
using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConfigurationTypeConfiguration : IEntityTypeConfiguration<ConfigurationType>
{
    public void Configure(EntityTypeBuilder<ConfigurationType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasData(
            new ConfigurationType
            {
                Id = Constants.ConfigurationTypes.EdiImport,
                Name = "EDI Import"
            }
        );
    }
}
