using UseCases;
using UseCases.Security.Roles;
using UseCases.Security.Roles.List;

namespace Infrastructure.Data.Queries.Security;

public class ListRolesQueryService(ReadDbContext db) : IListRolesQueryService
{
    public async Task<PagedResult<RoleDto>> ListAsync(int page, int perPage, CancellationToken ct)
    {
        var items = await db.Role
            .OrderBy(r => r.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(r => new RoleDto(r.Id, r.Name))
            .AsNoTracking()
            .ToListAsync(ct);

        int totalCount = await db.Role.CountAsync(ct);
        int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

        return new PagedResult<RoleDto>(items, page, perPage, totalCount, totalPages);
    }
}
