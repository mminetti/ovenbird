using Core.Common;
using Core.Security.Events;

namespace Core.Security;

public class User : AuditableEntityBase<int>
{
    public string ExternalIdentifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public ICollection<Role> Roles { get; set; } = [];

    public User UpdateName(string newName)
    {
        if (Name == newName) return this;
        Name = newName;
        RegisterDomainEvent(new UserNameUpdatedEvent(this));
        return this;
    }

    public User SetRoles(IEnumerable<Role> roles)
    {
        var roleList = roles.ToList();

        if (roleList.Count > 0)
        {
            var currentIds = Roles.Select(r => r.Id).ToHashSet();
            var newIds = roleList.Select(r => r.Id).ToHashSet();

            foreach (var role in roleList.Where(r => !currentIds.Contains(r.Id)))
            {
                Roles.Add(role);
            }

            foreach (var role in Roles.Where(r => !newIds.Contains(r.Id)).ToList())
            {
                Roles.Remove(role);
            }
        }
        else
        {
            Roles.Clear();
        }

        RegisterDomainEvent(new UserRolesUpdatedEvent(this));

        return this;
    }
}
