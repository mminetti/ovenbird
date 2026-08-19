using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Modules.Get;

public class GetModuleHandler(IReadRepository<Module> repository)
{
    public async Task<Result<ModuleDto>> Handle(GetModuleQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new ModuleByIdSpec(request.ModuleId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new ModuleDto(entity.Id, entity.Name));
    }
}
