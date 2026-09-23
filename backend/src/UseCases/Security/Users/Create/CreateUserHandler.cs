using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Users.Create;

public class CreateUserHandler(IRepository<User> repository, IRepository<Role> roleRepository)
{
    public async Task<Result<int>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var user = new User
        {
            Name = command.Name,
            Email = command.Email,
            ExternalIdentifier = command.ExternalIdentifier,
            IsActive = true
        };

        if (command.RoleIds.Count > 0)
        {
            var roles = await roleRepository.ListAsync(new RolesByIdsSpec(command.RoleIds), ct);

            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }
        }

        var created = await repository.AddAsync(user, ct);

        return created.Id;
    }
}
