using Core.Security;
using Core.Security.Events;

namespace UseCases.Security.Roles.Delete;

public class DeleteRoleHandler(IRepository<Role> repository, IMessageBus bus)
{
    public async Task<Result> Handle(DeleteRoleCommand command, CancellationToken ct)
    {
        var role = await repository.GetByIdAsync(command.RoleId, ct);

        if (role is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(role, ct);

        await bus.PublishAsync(new RoleDeletedEvent(role));

        return Result.Success();
    }
}
