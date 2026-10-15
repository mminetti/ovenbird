using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceStartTypeConfiguration : IEntityTypeConfiguration<ServiceStartType>
{
    public void Configure(EntityTypeBuilder<ServiceStartType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceStartType { Id = Constants.ServiceStartTypes.MoveIn, Name = nameof(Constants.ServiceStartTypes.MoveIn) },
            new ServiceStartType { Id = Constants.ServiceStartTypes.Switch, Name = nameof(Constants.ServiceStartTypes.Switch) });
    }
}
