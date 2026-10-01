using Web.Common;

namespace Web.Endpoints.Security.Permissions.List;

public sealed class ListPermissionsRequest : PagedRequest
{
    public const string Route = "/security/permissions";
}

public sealed class ListPermissionsValidator : PagedRequestValidator<ListPermissionsRequest>
{

}
