using UseCases;
using UseCases.Security.Modules;
using UseCases.Security.Modules.List;

namespace Infrastructure.Data.Queries.Security;

public class ListModulesQueryService(ReadDbContext db) : IListModulesQueryService
{
    public async Task<PagedResult<ModuleDto>> ListAsync(int page, int perPage, CancellationToken ct)
    {
        var items = await db.Module
            .OrderBy(m => m.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(m => new ModuleDto(m.Id, m.Name))
            .AsNoTracking()
            .ToListAsync(ct);

        int totalCount = await db.Module.CountAsync(ct);
        int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

        return new PagedResult<ModuleDto>(items, page, perPage, totalCount, totalPages);
    }
}
