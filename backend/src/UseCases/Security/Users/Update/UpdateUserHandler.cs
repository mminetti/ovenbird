using Core.Security;
using Core.Security.Events;

namespace UseCases.Security.Users.Update;

public class UpdateUserHandler(IRepository<User> repository, IMessageBus bus)
{
    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(command.UserId, ct);

        if (user is null)
        {
            return Result.NotFound();
        }

        user.UpdateName(command.Name);
        user.Email = command.Email;
        user.IsActive = command.IsActive;

        await repository.UpdateAsync(user, ct);

        await bus.PublishAsync(new UserUpdatedEvent(user));

        return Result.Success();
    }
}
