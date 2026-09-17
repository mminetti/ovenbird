using UseCases.Common;

namespace Web.Connectors.List;

public record ListConnectorsResponse : ItemPagedResult<ConnectorRecord>
{
    public ListConnectorsResponse(IReadOnlyList<ConnectorRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
