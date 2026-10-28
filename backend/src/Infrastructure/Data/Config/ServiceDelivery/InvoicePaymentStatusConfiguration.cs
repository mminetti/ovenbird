using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoicePaymentStatusConfiguration : IEntityTypeConfiguration<InvoicePaymentStatus>
{
    public void Configure(EntityTypeBuilder<InvoicePaymentStatus> builder) =>
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
}
