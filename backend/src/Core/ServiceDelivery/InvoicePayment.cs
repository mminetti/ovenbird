using Core.Common;

namespace Core.ServiceDelivery;

public class InvoicePayment : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public int UtilityId { get; set; }
    public decimal Amount { get; set; }
    public string TraceNumber { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public int InvoicePaymentStatusId { get; set; }

    public Utility Utility { get; set; } = default!;
    public InvoicePaymentStatus InvoicePaymentStatus { get; set; } = default!;
    public ICollection<Invoice> Invoices { get; set; } = [];
}
