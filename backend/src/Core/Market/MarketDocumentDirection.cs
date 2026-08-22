using Core.Common;

namespace Core.Market;

public class MarketDocumentDirection : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
}
