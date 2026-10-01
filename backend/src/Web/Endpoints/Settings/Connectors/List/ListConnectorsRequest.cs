using Web.Common;

namespace Web.Endpoints.Settings.Connectors.List;

public sealed class ListConnectorsRequest : PagedRequest
{
    public const string Route = "/settings/connectors";
}

public sealed class ListConnectorsValidator : PagedRequestValidator<ListConnectorsRequest>
{
}
