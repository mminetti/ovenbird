namespace UseCases.Security.Users.List;

public class ListUsersHandler(IListUsersQueryService query)
{
    public async Task<Result<PagedResult<UserDto>>> Handle(ListUsersQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.DEFAULT_PAGE_SIZE,
            ct);

        return Result.Success(result);
    }
}
