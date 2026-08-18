namespace UseCases.Users.List;

/// <summary>
/// Represents a service that fetches paged user data.
/// Typically implemented in Infrastructure.
/// </summary>
public interface IListUsersQueryService
{
    Task<PagedResult<UserDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
