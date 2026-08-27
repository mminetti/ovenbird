using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConfigurationFieldConfiguration : BaseEntityTypeConfiguration<ConfigurationField, int>
{
    public override void Configure(EntityTypeBuilder<ConfigurationField> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Value)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasOne(x => x.Configuration)
            .WithMany(x => x.ConfigurationFields)
            .HasForeignKey(x => x.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
