using UseCases.Common;

namespace Web.Security.Modules.List;

public record ListModulesResponse : ItemPagedResult<ModuleRecord>
{
    public ListModulesResponse(IReadOnlyList<ModuleRecord> Items, int Page, int PerPage, int TotalCount, int TotalPages)
        : base(Items, Page, PerPage, TotalCount, TotalPages)
    {
    }
}
