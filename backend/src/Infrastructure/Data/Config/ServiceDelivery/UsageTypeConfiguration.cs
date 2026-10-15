using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class UsageTypeConfiguration : IEntityTypeConfiguration<UsageType>
{
    public void Configure(EntityTypeBuilder<UsageType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new UsageType { Id = Constants.UsageTypes.NonInterval, Name = nameof(Constants.UsageTypes.NonInterval) },
            new UsageType { Id = Constants.UsageTypes.Interval, Name = nameof(Constants.UsageTypes.Interval) });
    }
}
