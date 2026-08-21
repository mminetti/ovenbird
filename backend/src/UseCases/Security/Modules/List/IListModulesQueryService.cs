using UseCases.Common;

namespace UseCases.Security.Modules.List;

public interface IListModulesQueryService
{
    Task<ItemPagedResult<ModuleDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
