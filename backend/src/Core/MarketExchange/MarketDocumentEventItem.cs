using Core.Common;

namespace Core.MarketExchange;

public class MarketDocumentEventItem : AuditableEntityBase<long>
{
    public long MarketDocumentEventId { get; set; }
    public int LevelId { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public MarketDocumentEvent MarketDocumentEvent { get; set; } = default!;
    public MarketDocumentEventItemLevel Level { get; set; } = default!;
}
