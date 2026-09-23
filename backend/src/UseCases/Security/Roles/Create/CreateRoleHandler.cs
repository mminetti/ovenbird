using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Roles.Create;

public class CreateRoleHandler(IRepository<Role> repository, IRepository<Permission> permissionRepository)
{
    public async Task<Result<int>> Handle(CreateRoleCommand command, CancellationToken ct)
    {
        var role = new Role
        {
            Name = command.Name
        };

        if (command.PermissionIds.Count > 0)
        {
            var permissions = await permissionRepository.ListAsync(new PermissionsByIdsSpec(command.PermissionIds), ct);

            foreach (var permission in permissions)
            {
                role.Permissions.Add(permission);
            }
        }

        var created = await repository.AddAsync(role, ct);

        return created.Id;
    }
}
