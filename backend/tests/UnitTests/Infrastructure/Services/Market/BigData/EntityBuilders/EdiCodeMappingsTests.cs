using Core.Common.Constants;
using Infrastructure.Services.Market.BigData.EntityBuilders;

namespace UnitTests.Infrastructure.Services.Market.BigData.EntityBuilders;

public class EdiCodeMappingsTests
{
    [Fact]
    public void ResolveCommodityIdMapsKnownCode()
    {
        EdiCodeMappings.ResolveCommodityId("E").ShouldBe(Constants.Commodities.Electricity);
    }

    [Fact]
    public void ResolveCommodityIdThrowsForUnknownCode()
    {
        Should.Throw<InvalidOperationException>(() => EdiCodeMappings.ResolveCommodityId("Z"));
    }

    [Fact]
    public void ResolveUnitOfMeasureIdMapsKnownCode()
    {
        EdiCodeMappings.ResolveUnitOfMeasureId("KH").ShouldBe(Constants.UnitOfMeasures.KilowattHour);
    }

    [Fact]
    public void ResolveUnitOfMeasureIdThrowsForUnknownCode()
    {
        Should.Throw<InvalidOperationException>(() => EdiCodeMappings.ResolveUnitOfMeasureId("ZZ"));
    }
}
