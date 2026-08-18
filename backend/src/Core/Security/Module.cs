using Core.Common;

namespace Core.Security;

public class Module : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = [];
}
