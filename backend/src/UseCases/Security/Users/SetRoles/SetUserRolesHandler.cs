using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Users.SetRoles;

public class SetUserRolesHandler(
    IRepository<User> userRepository,
    IReadRepository<Role> roleRepository)
{
    public async Task<Result> Handle(SetUserRolesCommand command, CancellationToken ct)
    {
        var user = await userRepository.FirstOrDefaultAsync(
            new UserWithRolesByIdSpec(command.UserId), ct);

        if (user is null)
        {
            return Result.NotFound();
        }

        if (command.RoleIds.Count > 0)
        {
            var roles = await roleRepository.ListAsync(
                new RolesByIdsSpec(command.RoleIds), ct);

            var missingIds = command.RoleIds
                .Except(roles.Select(r => r.Id))
                .ToList();

            if (missingIds.Count > 0)
            {
                return Result.Invalid(new ValidationError(
                    nameof(command.RoleIds),
                    $"The following role IDs do not exist: {string.Join(", ", missingIds)}."));
            }

            var currentIds = user.Roles.Select(r => r.Id).ToHashSet();
            var newIds = command.RoleIds.ToHashSet();

            foreach (var role in roles.Where(r => !currentIds.Contains(r.Id)))
            {
                user.Roles.Add(role);
            }

            foreach (var role in user.Roles.Where(r => !newIds.Contains(r.Id)).ToList())
            {
                user.Roles.Remove(role);
            }
        }
        else
        {
            user.Roles.Clear();
        }

        await userRepository.UpdateAsync(user, ct);

        return Result.Success();
    }
}
