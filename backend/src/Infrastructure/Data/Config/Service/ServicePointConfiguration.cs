using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class ServicePointConfiguration : BaseEntityTypeConfiguration<ServicePoint, long>
{
    public override void Configure(EntityTypeBuilder<ServicePoint> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
