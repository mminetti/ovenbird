using Core.Common;

namespace Core.Market;

public class MarketDocumentReference : AuditableEntityBase<long>
{
    public long MarketDocumentItemId { get; set; }
    public int ReferenceTypeId { get; set; } //historical usage, usage, invoice, service start, service end, service request
    public long ReferenceId { get; set; }

    public MarketDocumentItem MarketDocumentItem { get; set; } = default!;
}
