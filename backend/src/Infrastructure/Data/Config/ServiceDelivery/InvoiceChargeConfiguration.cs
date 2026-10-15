using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoiceChargeConfiguration : BaseEntityTypeConfiguration<InvoiceCharge, long>
{
    public override void Configure(EntityTypeBuilder<InvoiceCharge> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        // Cascade: charges are owned by the invoice, delete with it.
        builder.HasOne(x => x.Invoice)
            .WithMany()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.InvoiceChargeCode)
            .WithMany()
            .HasForeignKey(x => x.InvoiceChargeCodeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.InvoiceChargeType)
            .WithMany()
            .HasForeignKey(x => x.InvoiceChargeTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
