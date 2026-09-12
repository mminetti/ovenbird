using Core.Security;
using Infrastructure.Data;
using Infrastructure.Data.Queries.Security;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Data.Queries.Common;

// Exercises PagedQueryServiceBase's search/order logic through ListPermissionsQueryService,
// which was migrated off hand-rolled pagination onto the shared base + property mapper.
public class ListPermissionsQueryServiceListAsync : IDisposable
{
    private readonly ReadDbContext _db;
    private readonly ListPermissionsQueryService _service;

    public ListPermissionsQueryServiceListAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ReadDbContext(options);

        _db.Permission.AddRange(
            new Permission { Id = 1, Name = "users.read", Description = "Can read users" },
            new Permission { Id = 2, Name = "users.write", Description = "Can write users" },
            new Permission { Id = 3, Name = "roles.read", Description = "Can read roles" });

        _db.SaveChanges();

        _service = new ListPermissionsQueryService(_db, new PermissionQueryPropertyMapper());
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task OrdersByIdAscendingWhenNoOrderSpecified()
    {
        var result = await _service.ListAsync(1, 10, null, null, CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([1, 2, 3]);
    }

    [Fact]
    public async Task OrdersByMappedPropertyAscending()
    {
        var result = await _service.ListAsync(1, 10, null, "name", CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([3, 1, 2]);
    }

    [Fact]
    public async Task FiltersBySearchTermOnDescription()
    {
        var result = await _service.ListAsync(1, 10, "write", null, CancellationToken.None);

        result.TotalCount.ShouldBe(1);
        result.Items[0].Id.ShouldBe(2);
    }

    [Fact]
    public async Task FiltersAcrossNameAndDescription()
    {
        var result = await _service.ListAsync(1, 10, "users", null, CancellationToken.None);

        result.TotalCount.ShouldBe(2);
        result.Items.Select(x => x.Id).ShouldBe([1, 2]);
    }
}
