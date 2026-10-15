using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoiceTypeConfiguration : IEntityTypeConfiguration<InvoiceType>
{
    public void Configure(EntityTypeBuilder<InvoiceType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new InvoiceType { Id = Constants.InvoiceTypes.Month, Name = nameof(Constants.InvoiceTypes.Month) },
            new InvoiceType { Id = Constants.InvoiceTypes.Final, Name = nameof(Constants.InvoiceTypes.Final) },
            new InvoiceType { Id = Constants.InvoiceTypes.ChangeNotice, Name = nameof(Constants.InvoiceTypes.ChangeNotice) },
            new InvoiceType { Id = Constants.InvoiceTypes.BalanceDue, Name = nameof(Constants.InvoiceTypes.BalanceDue) },
            new InvoiceType { Id = Constants.InvoiceTypes.SecondNotice, Name = nameof(Constants.InvoiceTypes.SecondNotice) },
            new InvoiceType { Id = Constants.InvoiceTypes.Tampering, Name = nameof(Constants.InvoiceTypes.Tampering) },
            new InvoiceType { Id = Constants.InvoiceTypes.Charges, Name = nameof(Constants.InvoiceTypes.Charges) });
    }
}
