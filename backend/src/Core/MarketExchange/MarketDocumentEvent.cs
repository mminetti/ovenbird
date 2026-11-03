using Core.Common;

namespace Core.MarketExchange;

public class MarketDocumentEvent : AuditableEntityBase<long>
{
    public long MarketDocumentItemId { get; set; }
    public int EventTypeId { get; set; }
    public long? ReferenceId { get; set; }
    public int StatusId { get; set; }
    public string? Message { get; set; }

    public MarketDocumentItem MarketDocumentItem { get; set; } = default!;
    public MarketDocumentEventType EventType { get; set; } = default!;
    public MarketDocumentEventStatus Status { get; set; } = default!;
    public ICollection<MarketDocumentEventItem> Items { get; set; } = [];
}
