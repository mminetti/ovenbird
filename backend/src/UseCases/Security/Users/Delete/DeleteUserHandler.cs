using Core.Interfaces.Security;

namespace UseCases.Security.Users.Delete;

public class DeleteUserHandler(
    IDeleteUserService deleteUserService)
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        return await deleteUserService.DeleteUserAsync(command.UserId, ct);
    }
}
