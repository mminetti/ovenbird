using Ardalis.Result;
using Core.Common.Constants;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class BigDataValueMapperTests
{
    private readonly BigDataValueMapper _mapper = new();

    [Fact]
    public void GetCommodityIdMapsKnownCode()
    {
        _mapper.GetCommodityId("E").Value.ShouldBe(Constants.Commodities.Electricity);
    }

    [Fact]
    public void GetCommodityIdReturnsInvalidForUnknownCode()
    {
        _mapper.GetCommodityId("Z").Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public void GetUnitOfMeasureIdMapsKnownCode()
    {
        _mapper.GetUnitOfMeasureId("KH").Value.ShouldBe(Constants.UnitOfMeasures.KilowattHour);
    }

    [Fact]
    public void GetUnitOfMeasureIdReturnsInvalidForUnknownCode()
    {
        _mapper.GetUnitOfMeasureId("ZZ").Status.ShouldBe(ResultStatus.Invalid);
    }
}
