using System.Xml.Linq;
using Core.Common.Constants;
using Core.Service;
using Core.Subscription;
using Infrastructure.Services.Market.BigData.Builders;
using Infrastructure.Services.Market.BigData.Readers;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class SwitchRequestGeneratorCreateAsync
{
    private readonly SwitchRequest_814_01_Generator _generator = new(TimeProvider.System);

    [Fact]
    public async Task GeneratesA814_01TransactionThatRoundTripsThroughTheInboundReader()
    {
        var premise = new Premise { Id = 1, Address = "105 S ALAMO RD APT 15", City = "ALAMO", State = "TX", Zip = "78516" };
        var utility = new Utility { Id = 1, Name = "AEP Texas Central" };
        var customer = new Customer { Id = 1, FirstName = "Cynthia", LastName = "Alanis" };
        var servicePoint = new ServicePoint { Id = 1, PremiseId = premise.Id, Premise = premise, UtilityId = utility.Id, Utility = utility, IsCriticalCare = false };
        var account = new Account
        {
            Id = 1,
            Identifier = "10032789419071200",
            ServicePointId = servicePoint.Id,
            ServicePoint = servicePoint,
            CustomerId = customer.Id,
            Customer = customer,
        };
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
        item.ServicePointIdentifier.ShouldBe("10032789419071200");
        item.Raw.ShouldContain("<TransactionSubSet>01</TransactionSubSet>");
        item.Raw.ShouldContain("<State>TX</State>");
        item.Raw.ShouldContain("<UtilityName>AEP Texas Central</UtilityName>");
        item.Raw.ShouldContain("<EntityType>8R</EntityType>");
        item.Raw.ShouldContain("<EntityType>N1</EntityType>");
        item.Raw.ShouldContain("<EntityType>PO</EntityType>");
        item.Raw.ShouldContain("<EntityName>Cynthia Alanis</EntityName>");
        item.Raw.ShouldContain("<SpecialNeedsInfo>N</SpecialNeedsInfo>");
        item.Raw.ShouldContain("<ThirdServiceRequested>SW</ThirdServiceRequested>");
        item.Raw.ShouldContain("<SpecialMeterReadDate>20260115</SpecialMeterReadDate>");
        item.Raw.ShouldNotContain("PriorityCode");
        item.Raw.ShouldNotContain("MoveInDate");

        var roundTripped = new ServiceRequest_814_TransactionReader().Read(XElement.Parse(item.Raw));

        roundTripped.ReferenceNumber.ShouldBe("TXN-1");
        roundTripped.ServicePointIdentifier.ShouldBe("10032789419071200");
    }

    [Fact]
    public async Task FlagsCriticalCareAccountsAsSpecialNeeds()
    {
        var premise = new Premise { Id = 1, Address = "1 Main St", City = "Austin", State = "TX", Zip = "73301" };
        var utility = new Utility { Id = 1, Name = "AEP Texas Central" };
        var customer = new Customer { Id = 1, FirstName = "Jane", LastName = "Doe" };
        var servicePoint = new ServicePoint { Id = 1, PremiseId = premise.Id, Premise = premise, UtilityId = utility.Id, Utility = utility, IsCriticalCare = true };
        var account = new Account
        {
            Id = 1,
            Identifier = "ACCT-1",
            ServicePointId = servicePoint.Id,
            ServicePoint = servicePoint,
            CustomerId = customer.Id,
            Customer = customer,
        };
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

        item.Raw.ShouldContain("<SpecialNeedsInfo>Y</SpecialNeedsInfo>");
    }
}
