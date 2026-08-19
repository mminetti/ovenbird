using Core.Security;

namespace UseCases.Security.Modules.Delete;

public class DeleteModuleHandler(IRepository<Module> repository)
{
    public async Task<Result> Handle(DeleteModuleCommand command, CancellationToken ct)
    {
        var module = await repository.GetByIdAsync(command.ModuleId, ct);

        if (module is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(module, ct);

        return Result.Success();
    }
}
