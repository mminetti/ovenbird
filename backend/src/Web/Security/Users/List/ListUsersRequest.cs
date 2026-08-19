using Web.Common;

namespace Web.Security.Users.List;

public sealed class ListUsersRequest : PagedRequest
{
    public const string Route = "/security/users";
}

public sealed class ListUsersValidator : PagedRequestValidator<ListUsersRequest>
{

}
