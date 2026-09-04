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

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ConfigurationType)
            .WithMany(x => x.Configurations)
            .HasForeignKey(x => x.ConfigurationTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Company)
            .WithMany(x => x.Configurations)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        // Cascade: join table row only, never deletes either side.
        builder.HasMany(x => x.Connectors)
            .WithMany(x => x.Configurations)
            .UsingEntity<Dictionary<string, object>>(
                "ConfigurationConnector",
                j => j.HasOne<Connector>().WithMany().HasForeignKey("ConnectorId"),
                j => j.HasOne<Configuration>().WithMany().HasForeignKey("ConfigurationId"),
                j =>
                {
                    j.ToTable("ConfigurationConnector");
                    j.HasKey("ConfigurationId", "ConnectorId");
                });
    }
}
