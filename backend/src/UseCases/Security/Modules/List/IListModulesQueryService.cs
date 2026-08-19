namespace UseCases.Security.Modules.List;

public interface IListModulesQueryService
{
    Task<PagedResult<ModuleDto>> ListAsync(int page, int perPage, CancellationToken ct);
}
