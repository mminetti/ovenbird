namespace Core.Security.Events;

public sealed class UserNameUpdatedEvent(User user) : DomainEventBase
{
    public User User { get; set; } = user;
}
