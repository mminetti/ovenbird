using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Roles.Get;

public class GetRoleHandler(IReadRepository<Role> repository)
{
    public async Task<Result<RoleDto>> Handle(GetRoleQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new RoleByIdSpec(request.RoleId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new RoleDto(entity.Id, entity.Name));
    }
}
