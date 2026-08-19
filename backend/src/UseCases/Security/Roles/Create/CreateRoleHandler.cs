using Core.Security;

namespace UseCases.Security.Roles.Create;

public class CreateRoleHandler(IRepository<Role> repository)
{
    public async Task<Result<int>> Handle(CreateRoleCommand command, CancellationToken ct)
    {
        var role = new Role
        {
            Name = command.Name
        };

        var created = await repository.AddAsync(role, ct);

        return created.Id;
    }
}
