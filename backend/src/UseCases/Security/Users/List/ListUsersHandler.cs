namespace UseCases.Security.Users.List;

public class ListUsersHandler(IListUsersQueryService query)
{
    public async Task<Result<ItemPagedResult<UserDto>>> Handle(ListUsersQuery request, CancellationToken ct)
    {
        var result = await query.ListAsync(
            request.Page ?? 1,
            request.PerPage ?? Constants.Pagination.DefaultPageSize,
            request.Search,
            request.OrderBy,
            ct);

        return Result.Success(result);
    }
}
