using Core.Common;

namespace Core.ServiceDelivery;

public class InvoiceChargeCode : AuditableEntityBase<int>
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
