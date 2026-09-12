using Core.Market;
using Core.Security;
using Core.Shared;
using Infrastructure.Data;
using Infrastructure.Data.Queries.DataLists;
using Microsoft.EntityFrameworkCore;
using UseCases.DataLists;

namespace UnitTests.Infrastructure.Data.Queries.DataLists;

public class GetDataListQueryServiceGetListAsync : IDisposable
{
    private readonly ReadDbContext _db;
    private readonly GetDataListQueryService _service;

    public GetDataListQueryServiceGetListAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ReadDbContext(options);

        _db.Set<MarketDocumentDirection>().AddRange(
            new MarketDocumentDirection { Id = 2, Name = "Outbound" },
            new MarketDocumentDirection { Id = 1, Name = "Inbound" });

        _db.Set<ConnectorType>().AddRange(
            new ConnectorType { Id = 1, Name = "Ftp" },
            new ConnectorType { Id = 2, Name = "Api" });

        _db.Role.AddRange(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Editor" });

        _db.SaveChanges();

        _service = new GetDataListQueryService(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task OrdersFixedLookupsById()
    {
        var result = await _service.GetListAsync(DataListType.MarketDocumentDirections, CancellationToken.None);

        result.Select(x => x.Id).ShouldBe(["1", "2"]);
        result.Select(x => x.Name).ShouldBe(["Inbound", "Outbound"]);
    }

    [Fact]
    public async Task OrdersAdministeredLookupsByName()
    {
        var result = await _service.GetListAsync(DataListType.ConnectorTypes, CancellationToken.None);

        result.Select(x => x.Name).ShouldBe(["Api", "Ftp"]);
    }

    [Fact]
    public async Task ReturnsRolesOrderedByName()
    {
        var result = await _service.GetListAsync(DataListType.Roles, CancellationToken.None);

        result.Select(x => x.Id).ShouldBe(["1", "2"]);
        result.Select(x => x.Name).ShouldBe(["Admin", "Editor"]);
    }

    [Fact]
    public async Task ReturnsEmptyListWhenNoRowsExist()
    {
        var result = await _service.GetListAsync(DataListType.Markets, CancellationToken.None);

        result.ShouldBeEmpty();
    }
}
