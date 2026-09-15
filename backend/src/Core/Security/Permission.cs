using Core.Common;

namespace Core.Security;

public class Permission : EntityBase<int>, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Role> Roles { get; set; } = [];
}
