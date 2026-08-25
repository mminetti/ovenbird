using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class SystemIntegrationConfiguration : BaseEntityTypeConfiguration<SystemIntegration, int>
{
    public override void Configure(EntityTypeBuilder<SystemIntegration> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.SystemIntegrations)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
