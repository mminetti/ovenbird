using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Roles.SetPermissions;

public class SetRolePermissionsHandler(
    IRepository<Role> roleRepository,
    IReadRepository<Permission> permissionRepository)
{
    public async Task<Result> Handle(SetRolePermissionsCommand command, CancellationToken ct)
    {
        var role = await roleRepository.FirstOrDefaultAsync(new RoleWithPermissionsByIdSpec(command.RoleId), ct);

        if (role is null)
        {
            return Result.NotFound();
        }

        var permissions = new List<Permission>();

        if (command.PermissionIds.Count > 0)
        {
            permissions = await permissionRepository.ListAsync(new PermissionsByIdsSpec(command.PermissionIds), ct);
        }

        role.SetPermissions(permissions);

        await roleRepository.UpdateAsync(role, ct);

        return Result.Success();
    }
}
