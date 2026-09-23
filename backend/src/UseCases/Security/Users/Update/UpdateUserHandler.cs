using Core.Security;
using Core.Security.Events;
using Core.Security.Specifications;

namespace UseCases.Security.Users.Update;

public class UpdateUserHandler(
    IRepository<User> repository,
    IRepository<Role> roleRepository,
    IMessageBus bus)
{
    private const string Add = "add";
    private const string Remove = "remove";

    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await repository.FirstOrDefaultAsync(new UserWithRolesByIdSpec(command.UserId), ct);

        if (user is null)
        {
            return Result.NotFound();
        }

        user.UpdateName(command.Name);
        user.Email = command.Email;
        user.IsActive = command.IsActive;

        if (command.Roles is not null)
        {
            var roleIdsToAdd = command.Roles
                .Where(r => string.Equals(r.Operation, Add, StringComparison.OrdinalIgnoreCase))
                .Select(r => r.RoleId)
                .ToList();

            var rolesToAdd = roleIdsToAdd.Count > 0
                ? await roleRepository.ListAsync(new RolesByIdsSpec(roleIdsToAdd), ct)
                : [];

            foreach (var roleOperation in command.Roles)
            {
                switch (roleOperation.Operation.ToLowerInvariant())
                {
                    case Add:
                        var roleToAdd = rolesToAdd.FirstOrDefault(r => r.Id == roleOperation.RoleId);

                        if (roleToAdd is not null)
                        {
                            user.AddRole(roleToAdd);
                        }
                        break;

                    case Remove:
                        user.RemoveRole(roleOperation.RoleId);
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Invalid operation '{roleOperation.Operation}' for role with Id {roleOperation.RoleId}");
                }
            }
        }

        await repository.UpdateAsync(user, ct);

        await bus.PublishAsync(new UserUpdatedEvent(user));

        return Result.Success();
    }
}
