namespace Core.ServiceDelivery.Events;

public sealed class ServiceStartScheduledEvent(ServiceStart serviceStart, DateOnly serviceDate, long marketDocumentItemId) : DomainEventBase
{
    public ServiceStart ServiceStart { get; set; } = serviceStart;
    public DateOnly ServiceDate { get; set; } = serviceDate;
    public long MarketDocumentItemId { get; set; } = marketDocumentItemId;
}
