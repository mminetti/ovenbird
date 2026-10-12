using System.Xml.Linq;
using Core.Common.Constants;
using Core.Service;
using Core.Subscription;
using Infrastructure.Services.Market.BigData.Builders;
using Infrastructure.Services.Market.BigData.Readers;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class SwitchRequestGeneratorCreateAsync
{
    private readonly SwitchRequest_814_01_Generator _generator = new();

    [Fact]
    public async Task GeneratesA814_01TransactionThatRoundTripsThroughTheInboundReader()
    {
        var account = new Account { Id = 1, Identifier = "ACCT-123" };
        var serviceStart = new ServiceStart
        {
            Id = 1,
            AccountId = account.Id,
            Account = account,
            ServiceStartTypeId = Constants.ServiceStartTypes.Switch,
            ServiceDate = new DateOnly(2026, 1, 15),
        };
        var serviceStartItem = new ServiceStartItem
        {
            Id = 10,
            ServiceStartId = serviceStart.Id,
            ServiceStart = serviceStart,
            Identifier = "TXN-1",
            ServiceActionId = Constants.ServiceActions.Initiate,
        };

        var item = await _generator.CreateAsync(serviceStartItem, CancellationToken.None);

        item.ReferenceNumber.ShouldBe("TXN-1");
        item.ServicePointIdentifier.ShouldBe("ACCT-123");
        item.Raw.ShouldContain("<TransactionSubSet>01</TransactionSubSet>");

        var roundTripped = new ServiceRequest_814_TransactionReader().Read(XElement.Parse(item.Raw));

        roundTripped.ReferenceNumber.ShouldBe("TXN-1");
        roundTripped.ServicePointIdentifier.ShouldBe("ACCT-123");
    }
}
