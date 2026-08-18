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
}
