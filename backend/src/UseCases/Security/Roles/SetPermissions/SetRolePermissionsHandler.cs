using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Roles.SetPermissions;

public class SetRolePermissionsHandler(
    IRepository<Role> roleRepository,
    IReadRepository<Permission> permissionRepository)
{
    public async Task<Result> Handle(SetRolePermissionsCommand command, CancellationToken ct)
    {
        var role = await roleRepository.FirstOrDefaultAsync(
            new RoleWithPermissionsByIdSpec(command.RoleId), ct);

        if (role is null)
        {
            return Result.NotFound();
        }

        if (command.PermissionIds.Count > 0)
        {
            var permissions = await permissionRepository.ListAsync(
                new PermissionsByIdsSpec(command.PermissionIds), ct);

            var missingIds = command.PermissionIds
                .Except(permissions.Select(p => p.Id))
                .ToList();

            if (missingIds.Count > 0)
            {
                return Result.Invalid(new ValidationError(
                    nameof(command.PermissionIds),
                    $"The following permission IDs do not exist: {string.Join(", ", missingIds)}."));
            }

            var currentIds = role.Permissions.Select(p => p.Id).ToHashSet();
            var newIds = command.PermissionIds.ToHashSet();

            foreach (var permission in permissions.Where(p => !currentIds.Contains(p.Id)))
            {
                role.Permissions.Add(permission);
            }

            foreach (var permission in role.Permissions.Where(p => !newIds.Contains(p.Id)).ToList())
            {
                role.Permissions.Remove(permission);
            }
        }
        else
        {
            role.Permissions.Clear();
        }

        await roleRepository.UpdateAsync(role, ct);

        return Result.Success();
    }
}
