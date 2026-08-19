using Core.Security;

namespace UseCases.Security.Modules.Update;

public class UpdateModuleHandler(IRepository<Module> repository)
{
    public async Task<Result> Handle(UpdateModuleCommand command, CancellationToken ct)
    {
        var module = await repository.GetByIdAsync(command.ModuleId, ct);

        if (module is null)
        {
            return Result.NotFound();
        }

        module.Name = command.Name;

        await repository.UpdateAsync(module, ct);

        return Result.Success();
    }
}
