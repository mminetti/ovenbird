using Web.Common;

namespace Web.Security.Roles.List;

public sealed class ListRolesRequest : PagedRequest
{
    public const string Route = "/security/roles";
}

public sealed class ListRolesValidator : PagedRequestValidator<ListRolesRequest>
{

}
