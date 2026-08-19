using Core.Security;

namespace UseCases.Security.Modules.Create;

public class CreateModuleHandler(IRepository<Module> repository)
{
    public async Task<Result<int>> Handle(CreateModuleCommand command, CancellationToken ct)
    {
        var module = new Module
        {
            Name = command.Name
        };

        var created = await repository.AddAsync(module, ct);

        return created.Id;
    }
}
