using Core.Common;

namespace Core.Market;

public class MarketDocumentItemField : AuditableEntityBase<long>
{
    public long MarketDocumentItemId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldValue { get; set; } = string.Empty;

    public MarketDocumentItem MarketDocumentItem { get; set; } = default!;
}
