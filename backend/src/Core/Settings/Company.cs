using Core.Auditing;
using Core.Common;

namespace Core.Settings;

public class Company : AuditableEntityBase<int>, IAudited, IHasDisplayName
{
    public string Name { get; set; } = string.Empty;
    public int MarketId { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;

    public Market.Market Market { get; set; } = default!;
    public ICollection<Configuration> Configurations { get; set; } = [];
}
