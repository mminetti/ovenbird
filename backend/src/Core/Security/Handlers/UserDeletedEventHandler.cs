using Core.Security.Events;

namespace Core.Security.Handlers;

public class UserDeletedEventHandler(ILogger<UserDeletedEventHandler> logger)
{
    public async Task Handle(UserDeletedEvent domainEvent, CancellationToken ct)
    {
        logger.LogInformation("Handling User Deleted event for {userId}", domainEvent.UserId);

        await Task.CompletedTask;
    }
}
