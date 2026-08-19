using Core.Security;

namespace UseCases.Security.Roles.Delete;

public class DeleteRoleHandler(IRepository<Role> repository)
{
    public async Task<Result> Handle(DeleteRoleCommand command, CancellationToken ct)
    {
        var role = await repository.GetByIdAsync(command.RoleId, ct);

        if (role is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(role, ct);

        return Result.Success();
    }
}
