using Core.Common;
using Core.Service;
using Core.Settings;

namespace Core.Subscription;

public class Account : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public long CustomerId { get; set; }
    public int CompanyId { get; set; }
    public long ServicePointId { get; set; }
    public decimal Balance { get; set; }
    public DateOnly ServiceStartDate { get; set; }
    public DateOnly ServiceEndDate { get; set; }
    public int ServiceStatusId { get; set; } // pending, on, off
    public int ServiceSubStatusId { get; set; } // pending start, active, pending end, ended, closed
    public int ServiceConnectionStatusId { get; set; } // pending disconnection, disconnected, pending connection, connected
    public bool IsServiceLocked { get; set; }
    public bool IsServiceProtected { get; set; }
    public long ServiceStartId { get; set; }

    public Customer Customer { get; set; } = default!;
    public Company Company { get; set; } = default!;
    public ServicePoint ServicePoint { get; set; } = default!;
    public ServiceStatus ServiceStatus { get; set; } = default!;
    public ServiceSubStatus ServiceSubStatus { get; set; } = default!;
    public ServiceConnectionStatus ServiceConnectionStatus { get; set; } = default!;
    public ServiceStart ServiceStart { get; set; } = default!;
}
