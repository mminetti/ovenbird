using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConfigurationConfiguration : BaseEntityTypeConfiguration<Configuration, int>
{
    public override void Configure(EntityTypeBuilder<Configuration> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasOne(x => x.ConfigurationType)
            .WithMany(x => x.Configurations)
            .HasForeignKey(x => x.ConfigurationTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Configurations)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Connectors)
            .WithMany(x => x.Configurations);
    }
}
