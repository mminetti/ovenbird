using Core.Security;

namespace UseCases.Security.Permissions.Update;

public class UpdatePermissionHandler(IRepository<Permission> repository)
{
    public async Task<Result> Handle(UpdatePermissionCommand command, CancellationToken ct)
    {
        var permission = await repository.GetByIdAsync(command.PermissionId, ct);

        if (permission is null)
        {
            return Result.NotFound();
        }

        permission.ModuleId = command.ModuleId;
        permission.Name = command.Name;
        permission.Description = command.Description;

        await repository.UpdateAsync(permission, ct);

        return Result.Success();
    }
}
