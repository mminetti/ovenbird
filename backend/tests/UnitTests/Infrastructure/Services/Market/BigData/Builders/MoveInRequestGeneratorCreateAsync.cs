using System.Xml.Linq;
using Core.Common.Constants;
using Core.Service;
using Core.Subscription;
using Infrastructure.Services.Market.BigData.Builders;
using Infrastructure.Services.Market.BigData.Readers;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class MoveInRequestGeneratorCreateAsync
{
    private readonly MoveInRequest_814_16_Generator _generator = new(TimeProvider.System);

    [Fact]
    public async Task GeneratesA814_16TransactionThatRoundTripsThroughTheInboundReader()
    {
        var premise = new Premise { Id = 2, Address = "22 Oak Ave", City = "Houston", State = "TX", Zip = "77002" };
        var utility = new Utility { Id = 2, Name = "CenterPoint Energy" };
        var customer = new Customer { Id = 2, FirstName = "John", LastName = "Smith" };
        var servicePoint = new ServicePoint { Id = 2, Identifier = "ACCT-456", PremiseId = premise.Id, Premise = premise, UtilityId = utility.Id, Utility = utility, IsCriticalCare = false };
        var account = new Account
        {
            Id = 2,
            Identifier = "ACCOUNT-2",
            ServicePointId = servicePoint.Id,
            ServicePoint = servicePoint,
            CustomerId = customer.Id,
            Customer = customer,
        };
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
        item.Raw.ShouldContain("<UtilityName>CenterPoint Energy</UtilityName>");
        item.Raw.ShouldContain("<CustomerAgreementDate>20260201</CustomerAgreementDate>");
        item.Raw.ShouldContain("<MoveInDate>20260201</MoveInDate>");
        item.Raw.ShouldContain("<PriorityCode>01</PriorityCode>");
        item.Raw.ShouldNotContain("ThirdServiceRequested");
        item.Raw.ShouldNotContain("SpecialMeterReadDate");

        var roundTripped = new ServiceRequest_814_TransactionReader().Read(XElement.Parse(item.Raw));

        roundTripped.ReferenceNumber.ShouldBe("TXN-2");
        roundTripped.ServicePointIdentifier.ShouldBe("ACCT-456");
    }
}
