using UseCases.DataLists;
using UseCases.DataLists.Get;

namespace UnitTests.UseCases.DataLists;

public class GetDataListHandlerHandle
{
    private readonly IGetDataListQueryService _query = Substitute.For<IGetDataListQueryService>();
    private readonly GetDataListHandler _handler;

    public GetDataListHandlerHandle()
    {
        _handler = new GetDataListHandler(_query);
    }

    [Fact]
    public async Task ReturnsSuccessWithItemsAndTypeName()
    {
        var items = new List<ValuePairDto> { new("1", "Inbound"), new("2", "Outbound") };

        _query.GetListAsync(DataListType.MarketDocumentDirections, Arg.Any<CancellationToken>())
            .Returns(items);

        var result = await _handler.Handle(new GetDataListQuery(DataListType.MarketDocumentDirections), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Type.ShouldBe(nameof(DataListType.MarketDocumentDirections));
        result.Value.Items.ShouldBe(items);
    }
}
