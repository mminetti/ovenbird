using Core.Common;

namespace Core.Security;

public class Permission : AuditableEntityBase<int>
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Module Module { get; set; } = default!;
    public ICollection<Role> Roles { get; set; } = [];
}
