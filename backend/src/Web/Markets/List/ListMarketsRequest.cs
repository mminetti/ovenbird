using Web.Common;

namespace Web.Markets.List;

public sealed class ListMarketsRequest : PagedRequest
{
    public const string Route = "/markets";
}

public sealed class ListMarketsValidator : PagedRequestValidator<ListMarketsRequest>
{
}
