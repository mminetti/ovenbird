using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class PremiseConfiguration : BaseEntityTypeConfiguration<Premise, long>
{
    public override void Configure(EntityTypeBuilder<Premise> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.AddressLine1)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.City)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.State)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Zip)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Zip4)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.County)
            .WithMany()
            .HasForeignKey(x => x.CountyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
