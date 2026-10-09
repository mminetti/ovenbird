using Core.Common.Constants;
using Core.Service;
using Core.Service.Specifications;
using Infrastructure.Services.Market.BigData.EntityBuilders;

namespace UnitTests.Infrastructure.Services.Market.BigData.EntityBuilders;

public class BigDataLookupServiceTests
{
    private readonly IReadRepository<ServicePoint> _servicePointReadRepository = Substitute.For<IReadRepository<ServicePoint>>();
    private readonly IReadRepository<Meter> _meterReadRepository = Substitute.For<IReadRepository<Meter>>();
    private readonly BigDataLookupService _service;

    public BigDataLookupServiceTests()
    {
        _service = new BigDataLookupService(_servicePointReadRepository, _meterReadRepository);
    }

    [Fact]
    public void GetCommodityIdMapsKnownCode()
    {
        _service.GetCommodityId("E").ShouldBe(Constants.Commodities.Electricity);
    }

    [Fact]
    public void GetCommodityIdThrowsForUnknownCode()
    {
        Should.Throw<InvalidOperationException>(() => _service.GetCommodityId("Z"));
    }

    [Fact]
    public void GetUnitOfMeasureIdMapsKnownCode()
    {
        _service.GetUnitOfMeasureId("KH").ShouldBe(Constants.UnitOfMeasures.KilowattHour);
    }

    [Fact]
    public void GetUnitOfMeasureIdThrowsForUnknownCode()
    {
        Should.Throw<InvalidOperationException>(() => _service.GetUnitOfMeasureId("ZZ"));
    }

    [Fact]
    public async Task GetServicePointIdReturnsMatchingServicePointId()
    {
        _servicePointReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new ServicePoint { Id = 20, Identifier = "SP-1" });

        var id = await _service.GetServicePointId("SP-1", CancellationToken.None);

        id.ShouldBe(20);
    }

    [Fact]
    public async Task GetServicePointIdThrowsWhenIdentifierIsMissing()
    {
        await Should.ThrowAsync<InvalidOperationException>(
            () => _service.GetServicePointId(null, CancellationToken.None));
    }

    [Fact]
    public async Task GetServicePointIdThrowsWhenNotFound()
    {
        _servicePointReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServicePoint?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _service.GetServicePointId("SP-1", CancellationToken.None));
    }

    [Fact]
    public async Task GetMeterIdReturnsNullWhenIdentifierIsMissing()
    {
        var id = await _service.GetMeterId(null, CancellationToken.None);

        id.ShouldBeNull();
    }

    [Fact]
    public async Task GetMeterIdReturnsMatchingMeterId()
    {
        _meterReadRepository
            .FirstOrDefaultAsync(Arg.Any<MeterByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Meter { Id = 30, Identifier = "MTR-1" });

        var id = await _service.GetMeterId("MTR-1", CancellationToken.None);

        id.ShouldBe(30);
    }

    [Fact]
    public async Task GetMeterIdThrowsWhenIdentifierIsPresentButNotFound()
    {
        _meterReadRepository
            .FirstOrDefaultAsync(Arg.Any<MeterByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((Meter?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _service.GetMeterId("MTR-1", CancellationToken.None));
    }
}
