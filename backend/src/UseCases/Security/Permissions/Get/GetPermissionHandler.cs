using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Permissions.Get;

public class GetPermissionHandler(IReadRepository<Permission> repository)
{
    public async Task<Result<PermissionDto>> Handle(GetPermissionQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new PermissionByIdSpec(request.PermissionId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new PermissionDto(entity.Id, entity.ModuleId, entity.Name, entity.Description));
    }
}
