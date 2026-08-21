using UseCases.Common;

namespace UseCases.Security.Users.List;

public interface IListUsersQueryService
{
    Task<ItemPagedResult<UserDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
