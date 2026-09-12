using Core.Security;
using Infrastructure.Data;
using Infrastructure.Data.Queries.Security;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Data.Queries.Common;

// Exercises PagedQueryServiceBase's search/order logic through ListRolesQueryService,
// which was migrated off hand-rolled pagination onto the shared base + property mapper.
public class ListRolesQueryServiceListAsync : IDisposable
{
    private readonly ReadDbContext _db;
    private readonly ListRolesQueryService _service;

    public ListRolesQueryServiceListAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ReadDbContext(options);

        _db.Role.AddRange(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Editor" },
            new Role { Id = 3, Name = "Viewer" });

        _db.SaveChanges();

        _service = new ListRolesQueryService(_db, new RoleQueryPropertyMapper());
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task OrdersByIdAscendingWhenNoOrderSpecified()
    {
        var result = await _service.ListAsync(1, 10, null, null, CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([1, 2, 3]);
    }

    [Fact]
    public async Task OrdersByMappedPropertyDescending()
    {
        var result = await _service.ListAsync(1, 10, null, "name desc", CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([3, 2, 1]);
    }

    [Fact]
    public async Task FiltersBySearchTermOnName()
    {
        var result = await _service.ListAsync(1, 10, "Edit", null, CancellationToken.None);

        result.TotalCount.ShouldBe(1);
        result.Items[0].Id.ShouldBe(2);
    }
}
