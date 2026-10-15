using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new UnitOfMeasure { Id = Constants.UnitOfMeasures.KilowattHour, Name = "Kilowatt Hour", Identifier = "KH" });
    }
}
