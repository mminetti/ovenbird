using UseCases.Common;

namespace UnitTests.UseCases.Common;

public class EntityReferenceTests
{
    [Fact]
    public void FormatThenTryParse_RoundTrips()
    {
        var formatted = EntityReference.Format("MarketDocumentItem", 12345L);

        formatted.ShouldBe("MarketDocumentItem#12345");

        EntityReference.TryParse(formatted, out var entityType, out var entityId).ShouldBeTrue();
        entityType.ShouldBe("MarketDocumentItem");
        entityId.ShouldBe("12345");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NoSeparator")]
    [InlineData("#12345")]
    [InlineData("MarketDocumentItem#")]
    public void TryParse_ReturnsFalseForUnparseableValues(string? value)
    {
        EntityReference.TryParse(value, out var entityType, out var entityId).ShouldBeFalse();
        entityType.ShouldBeNull();
        entityId.ShouldBeNull();
    }
}
