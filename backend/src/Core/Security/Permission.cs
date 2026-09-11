using Core.Common;

namespace Core.Security;

public class Permission : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Role> Roles { get; set; } = [];
}
