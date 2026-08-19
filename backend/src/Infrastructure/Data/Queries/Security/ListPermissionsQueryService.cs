using UseCases;
using UseCases.Security.Permissions;
using UseCases.Security.Permissions.List;

namespace Infrastructure.Data.Queries.Security;

public class ListPermissionsQueryService(ReadDbContext db) : IListPermissionsQueryService
{
    public async Task<PagedResult<PermissionDto>> ListAsync(int page, int perPage, CancellationToken ct)
    {
        var items = await db.Permission
            .OrderBy(p => p.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(p => new PermissionDto(p.Id, p.ModuleId, p.Name, p.Description))
            .AsNoTracking()
            .ToListAsync(ct);

        int totalCount = await db.Permission.CountAsync(ct);
        int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

        return new PagedResult<PermissionDto>(items, page, perPage, totalCount, totalPages);
    }
}
