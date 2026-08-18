using Core.Security;
using Core.Security.Specifications;

namespace UseCases.Security.Users.Get;

public class GetUserHandler(IReadRepository<User> repository)
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new UserByIdSpec(request.UserId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new UserDto(entity.Id, entity.Name, entity.Email, entity.IsActive));
    }
}
