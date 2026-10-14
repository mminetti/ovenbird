using Core.Common;
using Core.Service;
using Core.Settings;

namespace Core.Subscription;

public class Account : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public long CustomerId { get; set; }
    public int CompanyId { get; set; }
    public decimal Balance { get; set; }

    public Customer Customer { get; set; } = default!;
    public Company Company { get; set; } = default!;
    public Service Service { get; set; } = default!;
    public ICollection<ServiceStart> ServiceStarts { get; set; } = [];
}
