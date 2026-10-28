using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class LoadProfileConfiguration : IEntityTypeConfiguration<LoadProfile>
{
    public void Configure(EntityTypeBuilder<LoadProfile> builder) =>
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
}
