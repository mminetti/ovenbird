using Core.Common;

namespace Core.Service;

public class ServiceStartItem : AuditableEntityBase<long>
{
    public long ServiceStartId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public DateOnly ServiceDate { get; set; }
    public int ServiceActionId { get; set; } // initiate, confirm, reject, complete, cancel, unexecute, error, change
    public string? Description { get; set; }

    public ServiceStart ServiceStart { get; set; } = default!;
    public ServiceAction ServiceAction { get; set; } = default!;
}
