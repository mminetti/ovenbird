using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Users.SetRoles;

public class SetUserRolesHandler(
    IRepository<User> userRepository,
    IReadRepository<Role> roleRepository)
{
    public async Task<Result> Handle(SetUserRolesCommand command, CancellationToken ct)
    {
        var user = await userRepository.FirstOrDefaultAsync(new UserWithRolesByIdSpec(command.UserId), ct);

        if (user is null)
        {
            return Result.NotFound();
        }

        var roles = new List<Role>();

        if (command.RoleIds.Count > 0)
        {
            roles = await roleRepository.ListAsync(new RolesByIdsSpec(command.RoleIds), ct);
        }

        user.SetRoles(roles);

        await userRepository.UpdateAsync(user, ct);

        return Result.Success();
    }
}
