namespace Core.MarketExchange.Events;

public sealed class MarketDocumentImportedEvent(MarketDocument document) : DomainEventBase
{
    public MarketDocument MarketDocument { get; } = document;
}
