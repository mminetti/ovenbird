using Core.Common;
using Core.Subscription;

namespace Core.ServiceDelivery;

public class Invoice : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public long ServiceId { get; set; }
    public long ServicePointId { get; set; }
    public int InvoicePurposeId { get; set; } // original, cancel, replace
    public int InvoiceTypeId { get; set; } // month, final, change notice, balance due, 2nd notice, tampering, charges
    public long? OriginalInvoiceId { get; set; }
    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }
    public bool IsFinal { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public int InvoiceStatusId { get; set; }

    public Service Service { get; set; } = default!;
    public ServicePoint ServicePoint { get; set; } = default!;
    public InvoicePurpose InvoicePurpose { get; set; } = default!;
    public InvoiceType InvoiceType { get; set; } = default!;
    public Invoice? OriginalInvoice { get; set; }
    public InvoiceStatus InvoiceStatus { get; set; } = default!;
}
