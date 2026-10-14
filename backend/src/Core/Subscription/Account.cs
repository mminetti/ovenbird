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
    public long ServiceStartId { get; set; }

    public Customer Customer { get; set; } = default!;
    public Company Company { get; set; } = default!;
    public ServicePoint ServicePoint { get; set; } = default!;
    public ServiceStart ServiceStart { get; set; } = default!;
    public Service.Service Service { get; set; } = default!;
}
