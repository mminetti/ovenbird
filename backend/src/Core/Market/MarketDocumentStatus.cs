using Core.Common;

namespace Core.Market;

public class MarketDocumentStatus : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
}
