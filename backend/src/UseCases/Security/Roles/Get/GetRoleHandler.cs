using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Permissions;

namespace UseCases.Security.Roles.Get;

public class GetRoleHandler(IReadRepository<Role> repository)
{
    public async Task<Result<RoleDto>> Handle(GetRoleQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new RoleWithPermissionsByIdSpec(request.RoleId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        var permissions = entity.Permissions
            .Select(p => new PermissionDto(p.Id, p.ModuleId, p.Name, p.Description))
            .ToList();

        return Result.Success(new RoleDto(entity.Id, entity.Name) { Permissions = permissions });
    }
}
