namespace Core.Security.Events;

public sealed class RoleDeletedEvent(Role role) : DomainEventBase
{
    public Role Role { get; set; } = role;
}
