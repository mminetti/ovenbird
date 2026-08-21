namespace Core.Security.Events;

public sealed class UserUpdatedEvent(User user) : DomainEventBase
{
    public User User { get; set; } = user;
}
