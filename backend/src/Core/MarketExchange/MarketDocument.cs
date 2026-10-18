using Core.Assets;
using Core.Common;
using Core.Settings;

namespace Core.MarketExchange;

public class MarketDocument : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public int DirectionId { get; set; }
    public int CompanyId { get; set; }
    public int StatusId { get; set; }
    public long? AssetId { get; set; }

    public MarketDocumentDirection Direction { get; set; } = default!;
    public Company Company { get; set; } = default!;
    public MarketDocumentStatus Status { get; set; } = default!;
    public Asset? Asset { get; set; }
}
