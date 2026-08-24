using Core.Common;

namespace Core.Shared;

public class Company : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public int MarketId { get; set; }

    public Market.Market Market { get; set; } = default!;
}
