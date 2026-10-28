using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceEventReferenceTypeConfiguration : IEntityTypeConfiguration<ServiceEventReferenceType>
{
    public void Configure(EntityTypeBuilder<ServiceEventReferenceType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceEventReferenceType { Id = Constants.ServiceEventReferenceTypes.ServiceStartItem, Name = nameof(Constants.ServiceEventReferenceTypes.ServiceStartItem) });
    }
}
