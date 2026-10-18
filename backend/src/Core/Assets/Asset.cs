using Core.Common;

namespace Core.Assets;

public class Asset : AuditableEntityBase<long>
{
    public string FileName { get; set; } = string.Empty;
    public string? Extension { get; set; }
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    public string Reference { get; set; } = string.Empty;
}
