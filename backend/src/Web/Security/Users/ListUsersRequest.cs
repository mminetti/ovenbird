namespace Web.Security.Users;

public sealed class ListUsersRequest
{
    [BindFrom("page")]
    public int Page { get; init; } = 1;

    [BindFrom("per_page")]
    public int PerPage { get; init; } = UseCases.Constants.DEFAULT_PAGE_SIZE;
}
