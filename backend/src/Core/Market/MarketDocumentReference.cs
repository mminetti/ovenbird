using Core.Common;

namespace Core.Market;

public class MarketDocumentReference : AuditableEntityBase<long>
{
    public long MarketDocumentItemId { get; set; }
    public int ReferenceTypeId { get; set; }
    public long ReferenceId { get; set; }

    public MarketDocumentItem MarketDocumentItem { get; set; } = default!;
    public MarketDocumentReferenceType ReferenceType { get; set; } = default!;
}
