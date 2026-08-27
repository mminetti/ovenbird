using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class IntegrationFieldConfiguration : BaseEntityTypeConfiguration<IntegrationField, int>
{
    public override void Configure(EntityTypeBuilder<IntegrationField> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Value)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasOne(x => x.Integration)
            .WithMany(x => x.IntegrationFields)
            .HasForeignKey(x => x.IntegrationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
