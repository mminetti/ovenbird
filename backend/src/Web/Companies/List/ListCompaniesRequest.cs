using Web.Common;

namespace Web.Companies.List;

public sealed class ListCompaniesRequest : PagedRequest
{
    public const string Route = "/companies";
}

public sealed class ListCompaniesValidator : PagedRequestValidator<ListCompaniesRequest>
{
}
