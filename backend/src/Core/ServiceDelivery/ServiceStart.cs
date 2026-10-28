using Core.Common;
using Core.Common.Constants;
using Core.ServiceDelivery.Events;
using Core.Subscription;

namespace Core.ServiceDelivery;

public class ServiceStart : AuditableEntityBase<long>
{
    public long ServiceId { get; set; }
    public int ServiceStartTypeId { get; set; } // move-in, switch
    public DateOnly ServiceDate { get; set; }
    public int ServiceOrderStatusId { get; set; } // new, in progress, scheduled, cancelled, rejected, completed, unexecuted, error

    public Service Service { get; set; } = default!;
    public ServiceStartType ServiceStartType { get; set; } = default!;
    public ServiceOrderStatus Status { get; set; } = default!;
    public ICollection<ServiceStartItem> Items { get; set; } = [];

    public void MarkScheduled(DateOnly serviceDate, ServiceStartScheduledEvent domainEvent)
    {
        ServiceDate = serviceDate;
        ServiceOrderStatusId = Constants.ServiceOrderStatuses.Scheduled;

        RegisterDomainEvent(domainEvent);
    }
}
