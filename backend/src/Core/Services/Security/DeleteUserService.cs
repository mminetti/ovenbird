using Core.Interfaces.Security;
using Core.Security;
using Core.Security.Events;

namespace Core.Services.Security;

public class DeleteUserService(
    ILogger<DeleteUserService> logger, 
    IRepository<User> repository,
    IMessageBus _bus) : IDeleteUserService
{
    public async Task<Result> DeleteUserAsync(int userId, CancellationToken ct)
    {
        logger.LogInformation("Deleting User {userId}", userId);

        User? user = await repository.GetByIdAsync(userId, ct);

        if (user is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(user, ct);

        var domainEvent = new UserDeletedEvent(user);

        await _bus.PublishAsync(domainEvent);

        return Result.Success();
    }
}
