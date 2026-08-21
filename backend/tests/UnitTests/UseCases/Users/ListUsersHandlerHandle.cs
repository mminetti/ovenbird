using UseCases.Common;
using UseCases.Security.Users;
using UseCases.Security.Users.List;

namespace UnitTests.UseCases.Users;

public class ListUsersHandlerHandle
{
    private readonly IListUsersQueryService _query = Substitute.For<IListUsersQueryService>();
    private readonly ListUsersHandler _handler;

    public ListUsersHandlerHandle()
    {
        _handler = new ListUsersHandler(_query);
    }

    [Fact]
    public async Task ReturnsSuccessWithPagedResults()
    {
        var users = new List<UserDto>
        {
            new(1, "Alice", "alice@example.com", true),
            new(2, "Bob", "bob@example.com", false),
        };
        var pagedResult = new ItemPagedResult<UserDto>(users, 1, 10, 2, 1);

        _query.ListAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _handler.Handle(new ListUsersQuery(1, 10), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.TotalCount.ShouldBe(2);
        result.Value.Page.ShouldBe(1);
    }

    [Fact]
    public async Task UsesDefaultsWhenPageAndPerPageAreNull()
    {
        var pagedResult = new ItemPagedResult<UserDto>([], 1, Constants.DEFAULT_PAGE_SIZE, 0, 0);

        _query.ListAsync(1, Constants.DEFAULT_PAGE_SIZE, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _handler.Handle(new ListUsersQuery(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _query.Received(1).ListAsync(1, Constants.DEFAULT_PAGE_SIZE, Arg.Any<CancellationToken>());
    }
}
