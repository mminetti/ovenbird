using Core.Security;

namespace UseCases.Security.Roles.Update;

public class UpdateRoleHandler(IRepository<Role> repository)
{
    public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken ct)
    {
        var role = await repository.GetByIdAsync(command.RoleId, ct);

        if (role is null)
        {
            return Result.NotFound();
        }

        role.Name = command.Name;

        await repository.UpdateAsync(role, ct);

        return Result.Success();
    }
}
