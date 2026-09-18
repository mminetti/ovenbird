using UseCases.Common;

namespace Web.Configurations.List;

public record ListConfigurationsResponse : ItemPagedResult<ConfigurationRecord>
{
    public ListConfigurationsResponse(IReadOnlyList<ConfigurationRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
