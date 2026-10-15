using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoicePurposeConfiguration : IEntityTypeConfiguration<InvoicePurpose>
{
    public void Configure(EntityTypeBuilder<InvoicePurpose> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new InvoicePurpose { Id = Constants.InvoicePurposes.Original, Name = nameof(Constants.InvoicePurposes.Original) },
            new InvoicePurpose { Id = Constants.InvoicePurposes.Cancel, Name = nameof(Constants.InvoicePurposes.Cancel) },
            new InvoicePurpose { Id = Constants.InvoicePurposes.Replace, Name = nameof(Constants.InvoicePurposes.Replace) });
    }
}
