using Core.Common;

namespace Core.Security;

public class Permission : EntityBase<int>, IAggregateRoot, IAudited
{
    public string Name { get; set; } = string.Empty;
    public int ModuleId { get; set; }
    public string Description { get; set; } = string.Empty;

    public PermissionModule Module { get; set; } = default!;
    public ICollection<Role> Roles { get; set; } = [];
}
