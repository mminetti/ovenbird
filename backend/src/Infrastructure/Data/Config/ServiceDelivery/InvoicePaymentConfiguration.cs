using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class InvoicePaymentConfiguration : BaseEntityTypeConfiguration<InvoicePayment, long>
{
    public override void Configure(EntityTypeBuilder<InvoicePayment> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.TraceNumber)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Utility)
            .WithMany()
            .HasForeignKey(x => x.UtilityId)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.InvoicePaymentStatus)
            .WithMany()
            .HasForeignKey(x => x.InvoicePaymentStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // Cascade: join table row only, never deletes either side.
        builder.HasMany(x => x.Invoices)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "InvoicePaymentInvoice",
                j => j.HasOne<Invoice>().WithMany().HasForeignKey("InvoiceId"),
                j => j.HasOne<InvoicePayment>().WithMany().HasForeignKey("InvoicePaymentId"),
                j =>
                {
                    j.ToTable("InvoicePaymentInvoice");
                    j.HasKey("InvoicePaymentId", "InvoiceId");
                });
    }
}
