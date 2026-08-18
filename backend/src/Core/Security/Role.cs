using Core.Common;

namespace Core.Security;

public class Role : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = [];
    public IList<Permission> Permissions { get; set; } = [];
}
