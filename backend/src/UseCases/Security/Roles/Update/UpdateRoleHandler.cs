using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Roles.Update;

public class UpdateRoleHandler(
    IRepository<Role> repository,
    IRepository<Permission> permissionRepository)
{
    private const string Add = "add";
    private const string Remove = "remove";

    public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken ct)
    {
        var role = await repository.FirstOrDefaultAsync(new RoleWithPermissionsByIdSpec(command.RoleId), ct);

        if (role is null)
        {
            return Result.NotFound();
        }

        role.Name = command.Name;

        if (command.Permissions is not null)
        {
            var permissionIdsToAdd = command.Permissions
                .Where(p => string.Equals(p.Operation, Add, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.PermissionId)
                .ToList();

            var permissionsToAdd = permissionIdsToAdd.Count > 0
                ? await permissionRepository.ListAsync(new PermissionsByIdsSpec(permissionIdsToAdd), ct)
                : [];

            foreach (var permissionOperation in command.Permissions)
            {
                switch (permissionOperation.Operation.ToLowerInvariant())
                {
                    case Add:
                        var permissionToAdd = permissionsToAdd.FirstOrDefault(p => p.Id == permissionOperation.PermissionId);

                        if (permissionToAdd is not null)
                        {
                            role.AddPermission(permissionToAdd);
                        }
                        break;

                    case Remove:
                        role.RemovePermission(permissionOperation.PermissionId);
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Invalid operation '{permissionOperation.Operation}' for permission with Id {permissionOperation.PermissionId}");
                }
            }
        }

        await repository.UpdateAsync(role, ct);

        return Result.Success();
    }
}
