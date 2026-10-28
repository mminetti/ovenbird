namespace Core.MarketExchange.Events;

public sealed class MarketDocumentItemImportedEvent(MarketDocumentItem item) : DomainEventBase
{
    public MarketDocumentItem MarketDocumentItem { get; } = item;
}
