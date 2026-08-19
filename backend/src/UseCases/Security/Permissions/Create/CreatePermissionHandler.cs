using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Permissions.Create;

public class CreatePermissionHandler(IRepository<Permission> repository, IReadRepository<Module> moduleRepository)
{
    public async Task<Result<int>> Handle(CreatePermissionCommand command, CancellationToken ct)
    {
        var module = await moduleRepository.FirstOrDefaultAsync(new ModuleByIdSpec(command.ModuleId), ct);

        if (module is null)
        {
            return Result<int>.Invalid(new ValidationError(nameof(command.ModuleId), "Module not found."));
        }

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
