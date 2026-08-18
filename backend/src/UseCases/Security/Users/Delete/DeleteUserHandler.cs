using Core.Security;

namespace UseCases.Security.Users.Delete;

public class DeleteUserHandler(IRepository<User> repository)
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(command.UserId, ct);

        if (user is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(user, ct);

        return Result.Success();
    }
}
