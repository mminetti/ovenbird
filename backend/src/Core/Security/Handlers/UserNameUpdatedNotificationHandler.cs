using Core.Security.Events;

namespace Core.Security.Handlers;

public class UserNameUpdatedNotificationHandler(ILogger<UserNameUpdatedNotificationHandler> logger)
{
    public async Task Handle(UserNameUpdatedEvent domainEvent, CancellationToken ct)
    {
        logger.LogInformation("Handling User Name Updated event for {userId}", domainEvent.User.Id);

        await Task.CompletedTask;
    }
}
