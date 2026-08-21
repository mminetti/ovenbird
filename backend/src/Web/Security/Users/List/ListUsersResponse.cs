using UseCases.Common;

namespace Web.Security.Users.List;

public record ListUsersResponse : ItemPagedResult<UserRecord>
{
    public ListUsersResponse(IReadOnlyList<UserRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
