using Web.Common;

namespace Web.Security.Modules.List;

public sealed class ListModulesRequest : PagedRequest
{
    public const string Route = "/security/modules";
}

public sealed class ListModulesValidator : PagedRequestValidator<ListModulesRequest>
{

}
