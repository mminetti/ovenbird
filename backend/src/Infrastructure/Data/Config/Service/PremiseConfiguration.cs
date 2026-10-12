using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class PremiseConfiguration : BaseEntityTypeConfiguration<Premise, long>
{
    public override void Configure(EntityTypeBuilder<Premise> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Address)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Address2)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.City)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.State)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Zip)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.County)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
