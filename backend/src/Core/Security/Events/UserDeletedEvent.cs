/// <summary>
/// A domain event that is dispatched whenever a user is deleted.
/// The DeleteUserService is used to dispatch this event.
/// </summary>
namespace Core.Security.Events;

public sealed class UserDeletedEvent(User user) : DomainEventBase
{
    public User User { get; set; } = user;
}
