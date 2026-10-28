using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceEventTypeConfiguration : IEntityTypeConfiguration<ServiceEventType>
{
    public void Configure(EntityTypeBuilder<ServiceEventType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceEventType { Id = Constants.ServiceEventTypes.ServiceStartConfirmed, Name = nameof(Constants.ServiceEventTypes.ServiceStartConfirmed) });
    }
}
