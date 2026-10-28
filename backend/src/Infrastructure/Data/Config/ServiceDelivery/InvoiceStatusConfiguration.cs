using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoiceStatusConfiguration : IEntityTypeConfiguration<InvoiceStatus>
{
    public void Configure(EntityTypeBuilder<InvoiceStatus> builder) =>
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
}
