using Core.Common;

namespace Core.MarketExchange;

public class MarketDocumentEventItem : ErrorLogItem
{
    public long MarketDocumentEventId { get; set; }

    public MarketDocumentEvent MarketDocumentEvent { get; set; } = default!;
}
