using Core.Interfaces.Security;
using Core.Security;
using Core.Security.Events;

namespace UseCases.Security.Users.Delete;

public class DeleteUserHandler(
    IDeleteUserService deleteUserService)
    //IRepository<User> repository,
    //IMessageBus _bus
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var result = await deleteUserService.DeleteUserAsync(command.UserId, ct);

        return result;
    }
}
