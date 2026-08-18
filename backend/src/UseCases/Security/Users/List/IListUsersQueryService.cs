namespace UseCases.Security.Users.List;

public interface IListUsersQueryService
{
    Task<PagedResult<UserDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
