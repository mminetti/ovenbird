using Core.Common;

namespace Core.ServiceDelivery;

public class ServiceEventAction : AuditableEntityBase<long>
{
    public long ServiceEventId { get; set; }
    public int ActionTypeId { get; set; }
    public int StatusId { get; set; }
    public string? Message { get; set; }

    public ServiceEvent ServiceEvent { get; set; } = default!;
    public ServiceEventActionType ActionType { get; set; } = default!;
    public ServiceOrderStatus Status { get; set; } = default!;
}
