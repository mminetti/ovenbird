using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class SystemIntegrationFieldConfiguration : BaseEntityTypeConfiguration<SystemIntegrationField, int>
{
    public override void Configure(EntityTypeBuilder<SystemIntegrationField> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Value)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasOne(x => x.SystemIntegration)
            .WithMany(x => x.SystemIntegrationFields)
            .HasForeignKey(x => x.SystemIntegrationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
