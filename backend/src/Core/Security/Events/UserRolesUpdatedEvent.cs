namespace Core.Security.Events;

public sealed class UserRolesUpdatedEvent(User user) : DomainEventBase
{
    public User User { get; set; } = user;
}
