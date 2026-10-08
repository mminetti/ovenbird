using Core.Common;

namespace Core.Market;

public class MarketDocumentItem : AuditableEntityBase<long>
{
    public long? MarketDocumentId { get; set; }
    public int MarketDocumentItemStatusId { get; set; }
    public string Set { get; set; } = string.Empty;
    public string SubSet {  get; set; } = string.Empty;
    public string Purpose {  get; set; } = string.Empty;
    public string SubPurpose { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public string? OriginalTrackingNumber {  get; set; } = string.Empty;
    public string ServicePointIdentifier {  get; set; } = string.Empty;
    public string Raw { get; set; } = string.Empty;

    public MarketDocument? MarketDocument { get; set; }
    public MarketDocumentItemStatus MarketDocumentItemStatus { get; set; } = default!;
    public ICollection<MarketDocumentItemField> Fields { get; set; } = [];
}
