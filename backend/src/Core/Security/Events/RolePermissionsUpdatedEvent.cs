namespace Core.Security.Events;

public sealed class RolePermissionsUpdatedEvent(Role role) : DomainEventBase
{
    public Role Role { get; set; } = role;
}
