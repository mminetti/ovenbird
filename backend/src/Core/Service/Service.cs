using Core.Common;
using Core.Subscription;

namespace Core.Service;

public class Service : AuditableEntityBase<long>
{
    public long AccountId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int StatusId { get; set; } // pending, on, off
    public int SubStatusId { get; set; } // pending start, active, pending end, ended, closed
    public int ConnectionStatusId { get; set; } // pending disconnection, disconnected, pending connection, connected
    public bool IsLocked { get; set; }
    public bool IsProtected { get; set; }

    public Account Account { get; set; } = default!;
    public ServiceStatus Status { get; set; } = default!;
    public ServiceSubStatus SubStatus { get; set; } = default!;
    public ServiceConnectionStatus ConnectionStatus { get; set; } = default!;
}
