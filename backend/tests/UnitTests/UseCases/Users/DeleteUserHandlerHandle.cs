using Ardalis.Result;
using Core.Interfaces.Security;
using UseCases.Security.Users.Delete;

namespace UnitTests.UseCases.Users;

public class DeleteUserHandlerHandle
{
    private readonly IDeleteUserService _deleteUserService = Substitute.For<IDeleteUserService>();
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerHandle()
    {
        _handler = new DeleteUserHandler(_deleteUserService);
    }

    [Fact]
    public async Task ReturnsSuccessWhenUserExists()
    {
        _deleteUserService.DeleteUserAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await _handler.Handle(new DeleteUserCommand(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _deleteUserService.Received(1).DeleteUserAsync(1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenUserDoesNotExist()
    {
        _deleteUserService.DeleteUserAsync(999, Arg.Any<CancellationToken>())
            .Returns(Result.NotFound());

        var result = await _handler.Handle(new DeleteUserCommand(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
    }
}
