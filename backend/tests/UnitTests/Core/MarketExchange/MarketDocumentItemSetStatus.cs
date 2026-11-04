using Core.Common.Constants;
using Core.MarketExchange;

namespace UnitTests.Core.MarketExchange;

public class MarketDocumentItemSetStatus
{
    private static MarketDocumentItem CreateValidItem() => new()
    {
        Set = "814",
        SubSet = "05",
        ReferenceNumber = "REF-1",
        ServicePointIdentifier = "SP-1",
    };

    [Fact]
    public void SetsNewWhenAllRequiredFieldsArePresent()
    {
        var item = CreateValidItem();

        item.SetStatus();

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.New);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetsErrorWhenSetIsMissing(string? set)
    {
        var item = CreateValidItem();
        item.Set = set!;

        item.SetStatus();

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetsErrorWhenSubSetIsMissing(string? subSet)
    {
        var item = CreateValidItem();
        item.SubSet = subSet!;

        item.SetStatus();

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetsErrorWhenReferenceNumberIsMissing(string? referenceNumber)
    {
        var item = CreateValidItem();
        item.ReferenceNumber = referenceNumber!;

        item.SetStatus();

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetsErrorWhenServicePointIdentifierIsMissing(string? servicePointIdentifier)
    {
        var item = CreateValidItem();
        item.ServicePointIdentifier = servicePointIdentifier!;

        item.SetStatus();

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);
    }
}
