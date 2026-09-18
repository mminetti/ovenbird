using Web.Common;

namespace Web.Configurations.List;

public sealed class ListConfigurationsRequest : PagedRequest
{
    public const string Route = "/configurations";
}

public sealed class ListConfigurationsValidator : PagedRequestValidator<ListConfigurationsRequest>
{
}
