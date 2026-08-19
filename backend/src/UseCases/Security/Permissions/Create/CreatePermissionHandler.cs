using Core.Security;

namespace UseCases.Security.Permissions.Create;

public class CreatePermissionHandler(IRepository<Permission> repository)
{
    public async Task<Result<int>> Handle(CreatePermissionCommand command, CancellationToken ct)
    {
        var permission = new Permission
        {
            ModuleId = command.ModuleId,
            Name = command.Name,
            Description = command.Description
        };

        var created = await repository.AddAsync(permission, ct);

        return created.Id;
    }
}
