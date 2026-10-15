using Core.Common;

namespace Core.ServiceDelivery;

public class InvoiceCharge : AuditableEntityBase<long>
{
    public long InvoiceId { get; set; }
    public int InvoiceChargeCodeId { get; set; }
    public int InvoiceChargeTypeId { get; set; } // rate, account, b2b
    public double Quantity { get; set; }
    public double ChargeAmount { get; set; }
    public double TotalAmount { get; set; }
    public int UnitOfMeasureId { get; set; }
    public string? Description { get; set; }

    public Invoice Invoice { get; set; } = default!;
    public InvoiceChargeCode InvoiceChargeCode { get; set; } = default!;
    public InvoiceChargeType InvoiceChargeType { get; set; } = default!;
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;
}
