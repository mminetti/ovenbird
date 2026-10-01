using Web.Common;

namespace Web.Endpoints.Security.Users.List;

public sealed class ListUsersRequest : PagedRequest
{
    public const string Route = "/security/users";
}
