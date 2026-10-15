using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoiceConfiguration : BaseEntityTypeConfiguration<Invoice, long>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ServicePoint)
            .WithMany()
            .HasForeignKey(x => x.ServicePointId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.InvoicePurpose)
            .WithMany()
            .HasForeignKey(x => x.InvoicePurposeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.InvoiceType)
            .WithMany()
            .HasForeignKey(x => x.InvoiceTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.InvoiceStatus)
            .WithMany()
            .HasForeignKey(x => x.InvoiceStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: self-referencing FK; SQL Server also rejects cascade on self-references.
        builder.HasOne(x => x.OriginalInvoice)
            .WithMany()
            .HasForeignKey(x => x.OriginalInvoiceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
