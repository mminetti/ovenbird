using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class MeterConfiguration : BaseEntityTypeConfiguration<Meter, long>
{
    public override void Configure(EntityTypeBuilder<Meter> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
