using UseCases;
using UseCases.Security.Users;
using UseCases.Security.Users.List;

namespace Infrastructure.Data.Queries.Security;

public class ListUsersQueryService(ReadDbContext db) : IListUsersQueryService
{
    public async Task<PagedResult<UserDto>> ListAsync(int page, int perPage, CancellationToken ct)
    {
        var items = await db.User
            .OrderBy(u => u.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsActive))
            .AsNoTracking()
            .ToListAsync(ct);

        int totalCount = await db.User.CountAsync(ct);
        int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

        return new PagedResult<UserDto>(items, page, perPage, totalCount, totalPages);
    }
}
