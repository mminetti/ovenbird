using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Roles;

namespace UseCases.Security.Users.Get;

public class GetUserHandler(IReadRepository<User> repository)
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new UserWithRolesByIdSpec(request.UserId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        var roles = entity.Roles
            .Select(r => new RoleDto(r.Id, r.Name))
            .ToList();

        return Result.Success(new UserDto(entity.Id, entity.Name, entity.Email, entity.IsActive) { Roles = roles });
    }
}
