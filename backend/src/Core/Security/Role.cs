using Core.Common;
using Core.Security.Events;

namespace Core.Security;

public class Role : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Permission> Permissions { get; set; } = [];

    public Role SetPermissions(IEnumerable<Permission> permissions)
    {
        var permissionsList = permissions.ToList();

        if (permissionsList.Count > 0)
        {
            var currentIds = Permissions.Select(x => x.Id).ToHashSet();
            var newIds = permissionsList.Select(x => x.Id).ToHashSet();

            foreach (var permission in permissionsList.Where(x => !currentIds.Contains(x.Id)))
            {
                Permissions.Add(permission);
            }

            foreach (var permission in Permissions.Where(x => !newIds.Contains(x.Id)).ToList())
            {
                Permissions.Remove(permission);
            }
        }
        else
        {
            Permissions.Clear();
        }

        RegisterDomainEvent(new RolePermissionsUpdatedEvent(this));

        return this;
    }
}
