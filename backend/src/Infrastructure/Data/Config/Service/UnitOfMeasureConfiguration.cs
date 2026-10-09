using Core.Common.Constants;
using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new UnitOfMeasure { Id = Constants.UnitOfMeasures.KilowattHour, Name = "Kilowatt Hour" });
    }
}
