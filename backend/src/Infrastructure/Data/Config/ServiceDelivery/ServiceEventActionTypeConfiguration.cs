using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceEventActionTypeConfiguration : IEntityTypeConfiguration<ServiceEventActionType>
{
    public void Configure(EntityTypeBuilder<ServiceEventActionType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceEventActionType { Id = Constants.ServiceEventActionTypes.UpdateServicePoint, Name = nameof(Constants.ServiceEventActionTypes.UpdateServicePoint) },
            new ServiceEventActionType { Id = Constants.ServiceEventActionTypes.SendNotification, Name = nameof(Constants.ServiceEventActionTypes.SendNotification) });
    }
}
