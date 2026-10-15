using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoiceChargeTypeConfiguration : IEntityTypeConfiguration<InvoiceChargeType>
{
    public void Configure(EntityTypeBuilder<InvoiceChargeType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new InvoiceChargeType { Id = Constants.InvoiceChargeTypes.Rate, Name = nameof(Constants.InvoiceChargeTypes.Rate) },
            new InvoiceChargeType { Id = Constants.InvoiceChargeTypes.Account, Name = nameof(Constants.InvoiceChargeTypes.Account) },
            new InvoiceChargeType { Id = Constants.InvoiceChargeTypes.B2B, Name = nameof(Constants.InvoiceChargeTypes.B2B) });
    }
}
