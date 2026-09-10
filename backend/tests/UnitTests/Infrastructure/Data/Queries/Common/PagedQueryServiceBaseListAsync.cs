using Core.Security;
using Infrastructure.Data;
using Infrastructure.Data.Queries.Security;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Data.Queries.Common;

// Exercises PagedQueryServiceBase's search/order/pagination logic through the
// concrete ListUsersQueryService + UserQueryPropertyMapper, since the base
// class has no state of its own worth testing in isolation.
public class PagedQueryServiceBaseListAsync : IDisposable
{
    private readonly ReadDbContext _db;
    private readonly ListUsersQueryService _service;

    public PagedQueryServiceBaseListAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ReadDbContext(options);

        _db.User.AddRange(
            new User { Id = 1, ExternalIdentifier = "ext-1", Name = "Zoe Adams", Email = "zoe.adams@homeutil.com", IsActive = true },
            new User { Id = 2, ExternalIdentifier = "ext-2", Name = "Mike Brooks", Email = "mike.brooks@homeutil.com", IsActive = false },
            new User { Id = 3, ExternalIdentifier = "ext-3", Name = "Alice Carter", Email = "alice.carter@greenenergy.com", IsActive = true },
            new User { Id = 4, ExternalIdentifier = "ext-4", Name = "Carol Davis", Email = "carol.davis@greenenergy.com", IsActive = true },
            new User { Id = 5, ExternalIdentifier = "ext-5", Name = "Bob Evans", Email = "bob.evans@homeutil.com", IsActive = false });

        _db.SaveChanges();

        _service = new ListUsersQueryService(_db, new UserQueryPropertyMapper());
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task ReturnsFirstPageWithDefaultPageSize()
    {
        var result = await _service.ListAsync(1, 10, null, null, CancellationToken.None);

        result.Items.Count.ShouldBe(5);
        result.TotalCount.ShouldBe(5);
        result.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task OrdersByIdAscendingWhenNoOrderSpecified()
    {
        var result = await _service.ListAsync(1, 10, null, null, CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([1, 2, 3, 4, 5]);
    }

    [Fact]
    public async Task PaginatesAcrossPagesCorrectly()
    {
        var page1 = await _service.ListAsync(1, 2, null, null, CancellationToken.None);
        var page3 = await _service.ListAsync(3, 2, null, null, CancellationToken.None);

        page1.Items.Select(x => x.Id).ShouldBe([1, 2]);
        page3.Items.Select(x => x.Id).ShouldBe([5]);
        page1.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task OrdersByMappedPropertyAscending()
    {
        var result = await _service.ListAsync(1, 10, null, "name", CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([3, 5, 4, 2, 1]);
    }

    [Fact]
    public async Task OrdersByMappedPropertyDescending()
    {
        var result = await _service.ListAsync(1, 10, null, "name desc", CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([1, 2, 4, 5, 3]);
    }

    [Fact]
    public async Task FallsBackToDefaultOrderingWhenOrderByPropertyIsNotMapped()
    {
        var result = await _service.ListAsync(1, 10, null, "externalIdentifier", CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([1, 2, 3, 4, 5]);
    }

    [Fact]
    public async Task FiltersBySearchTermOnName()
    {
        var result = await _service.ListAsync(1, 10, "Carter", null, CancellationToken.None);

        result.TotalCount.ShouldBe(1);
        result.Items[0].Id.ShouldBe(3);
    }

    [Fact]
    public async Task FiltersAcrossAllSearchableProperties()
    {
        var result = await _service.ListAsync(1, 10, "greenenergy", null, CancellationToken.None);

        result.TotalCount.ShouldBe(2);
        result.Items.Select(x => x.Id).ShouldBe([3, 4]);
    }

    [Fact]
    public async Task TotalCountReflectsFilterBeforePagination()
    {
        var result = await _service.ListAsync(1, 1, "homeutil", null, CancellationToken.None);

        result.Items.Count.ShouldBe(1);
        result.TotalCount.ShouldBe(3);
        result.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task ReturnsEmptyListWhenSearchMatchesNothing()
    {
        var result = await _service.ListAsync(1, 10, "doesnotexist", null, CancellationToken.None);

        result.Items.Count.ShouldBe(0);
        result.TotalCount.ShouldBe(0);
        result.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task CombinesSearchOrderingAndPagination()
    {
        var result = await _service.ListAsync(1, 2, "homeutil", "name", CancellationToken.None);

        result.Items.Select(x => x.Id).ShouldBe([5, 2]);
        result.TotalCount.ShouldBe(3);
        result.TotalPages.ShouldBe(2);
    }
}
