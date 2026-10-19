using Core.Common.Constants;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class BigDataValueMapperTests
{
    private readonly BigDataValueMapper _mapper = new();

    [Fact]
    public void GetCommodityIdMapsKnownCode()
    {
        _mapper.GetCommodityId("E").ShouldBe(Constants.Commodities.Electricity);
    }

    [Fact]
    public void GetCommodityIdThrowsForUnknownCode()
    {
        Should.Throw<InvalidOperationException>(() => _mapper.GetCommodityId("Z"));
    }

    [Fact]
    public void GetUnitOfMeasureIdMapsKnownCode()
    {
        _mapper.GetUnitOfMeasureId("KH").ShouldBe(Constants.UnitOfMeasures.KilowattHour);
    }

    [Fact]
    public void GetUnitOfMeasureIdThrowsForUnknownCode()
    {
        Should.Throw<InvalidOperationException>(() => _mapper.GetUnitOfMeasureId("ZZ"));
    }
}
