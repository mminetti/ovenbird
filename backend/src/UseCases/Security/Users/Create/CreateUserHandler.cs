using Core.Security;

namespace UseCases.Security.Users.Create;

public class CreateUserHandler(IRepository<User> repository)
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

        var created = await repository.AddAsync(user, ct);

        return created.Id;
    }
}
