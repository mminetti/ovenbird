using Core.Common;

namespace Core.Service;

public class Meter : AuditableEntityBase<long>
{
    public long ServicePointId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public ServicePoint ServicePoint { get; set; } = default!;
}
