namespace Web.Security.Users;

public record ListUsersResponse : UseCases.PagedResult<UserRecord>
{
    public ListUsersResponse(IReadOnlyList<UserRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
