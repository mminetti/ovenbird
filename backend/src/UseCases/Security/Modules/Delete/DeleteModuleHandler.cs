using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Modules.Delete;

public class DeleteModuleHandler(IRepository<Module> repository)
{
    public async Task<Result> Handle(DeleteModuleCommand command, CancellationToken ct)
    {
        var module = await repository.FirstOrDefaultAsync(new ModuleWithPermissionsByIdSpec(command.ModuleId), ct);

        if (module is null)
        {
            return Result.NotFound();
        }

        if (module.Permissions.Count > 0)
        {
            return Result.Conflict("Module has associated permissions and cannot be deleted.");
        }

        await repository.DeleteAsync(module, ct);

        return Result.Success();
    }
}
