using Core.Common;
using Core.Subscription;

namespace Core.Service;

public class ServiceStart : AuditableEntityBase<long>
{
    public long AccountId { get; set; }
    public int ServiceStartTypeId { get; set; } // move-in, switch
    public DateOnly ServiceDate { get; set; }
    public int ServiceOrderStatusId { get; set; } // new, in progress, scheduled, cancelled, rejected, completed, unexecuted, error

    public Account Account { get; set; } = default!;
    public ServiceStartType ServiceStartType { get; set; } = default!;
    public ServiceOrderStatus ServiceStatus { get; set; } = default!;
    public ICollection<ServiceStartItem> Items { get; set; } = [];
}
