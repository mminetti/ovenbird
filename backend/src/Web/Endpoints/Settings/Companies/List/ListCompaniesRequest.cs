using Web.Common;

namespace Web.Endpoints.Settings.Companies.List;

public sealed class ListCompaniesRequest : PagedRequest
{
    public const string Route = "/settings/companies";
}

public sealed class ListCompaniesValidator : PagedRequestValidator<ListCompaniesRequest>
{
}
