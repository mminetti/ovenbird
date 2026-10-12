using System.Xml.Linq;
using Core.Common.Constants;
using Core.Service;
using Core.Subscription;
using Infrastructure.Services.Market.BigData.Builders;
using Infrastructure.Services.Market.BigData.Readers;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class MoveInRequestGeneratorCreateAsync
{
    private readonly MoveInRequest_814_16_Generator _generator = new();

    [Fact]
    public async Task GeneratesA814_16TransactionThatRoundTripsThroughTheInboundReader()
    {
        var account = new Account { Id = 2, Identifier = "ACCT-456" };
        var serviceStart = new ServiceStart
        {
            Id = 2,
            AccountId = account.Id,
            Account = account,
            ServiceStartTypeId = Constants.ServiceStartTypes.MoveIn,
            ServiceDate = new DateOnly(2026, 2, 1),
        };
        var serviceStartItem = new ServiceStartItem
        {
            Id = 20,
            ServiceStartId = serviceStart.Id,
            ServiceStart = serviceStart,
            Identifier = "TXN-2",
            ServiceActionId = Constants.ServiceActions.Initiate,
        };

        var item = await _generator.CreateAsync(serviceStartItem, CancellationToken.None);

        item.ReferenceNumber.ShouldBe("TXN-2");
        item.ServicePointIdentifier.ShouldBe("ACCT-456");
        item.Raw.ShouldContain("<TransactionSubSet>16</TransactionSubSet>");

        var roundTripped = new ServiceRequest_814_TransactionReader().Read(XElement.Parse(item.Raw));

        roundTripped.ReferenceNumber.ShouldBe("TXN-2");
        roundTripped.ServicePointIdentifier.ShouldBe("ACCT-456");
    }
}
