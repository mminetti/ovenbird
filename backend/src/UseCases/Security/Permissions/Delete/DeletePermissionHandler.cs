using Core.Security;

namespace UseCases.Security.Permissions.Delete;

public class DeletePermissionHandler(IRepository<Permission> repository)
{
    public async Task<Result> Handle(DeletePermissionCommand command, CancellationToken ct)
    {
        var permission = await repository.GetByIdAsync(command.PermissionId, ct);

        if (permission is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(permission, ct);

        return Result.Success();
    }
}
