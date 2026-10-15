using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoiceChargeCodeConfiguration : BaseEntityTypeConfiguration<InvoiceChargeCode, int>
{
    public override void Configure(EntityTypeBuilder<InvoiceChargeCode> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
