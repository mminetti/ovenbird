using Core.Common;
using Core.Shared;

namespace Core.Market;

public class MarketDocument : AuditableEntityBase<long>
{
    public string Name { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public int DirectionId { get; set; }
    public int CompanyId { get; set; }
    public int StatusId { get; set; }

    public MarketDocumentDirection Direction { get; set; } = default!;
    public Company Company { get; set; } = default!;
    public MarketDocumentStatus Status { get; set; } = default!;
}
