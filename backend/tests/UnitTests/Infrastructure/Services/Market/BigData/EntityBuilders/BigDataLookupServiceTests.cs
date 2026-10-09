using Core.Common.Constants;
using Core.Service;
using Core.Service.Specifications;
using Infrastructure.Services.Market.BigData.EntityBuilders;

namespace UnitTests.Infrastructure.Services.Market.BigData.EntityBuilders;

public class BigDataLookupServiceTests
{
    private readonly IReadRepository<ServicePoint> _servicePointReadRepository = Substitute.For<IReadRepository<ServicePoint>>();
    private readonly BigDataLookupService _service;

    public BigDataLookupServiceTests()
    {
        _service = new BigDataLookupService(_servicePointReadRepository);
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
    public async Task GetServicePointAsyncReturnsMatchingServicePoint()
    {
        var servicePoint = new ServicePoint { Id = 20, Identifier = "SP-1" };

        _servicePointReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(servicePoint);

        var result = await _service.GetServicePointAsync("SP-1", CancellationToken.None);

        result.ShouldBe(servicePoint);
    }

    [Fact]
    public async Task GetServicePointAsyncThrowsWhenNotFound()
    {
        _servicePointReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServicePoint?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _service.GetServicePointAsync("SP-1", CancellationToken.None));
    }
}
