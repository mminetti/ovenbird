using Microsoft.AspNetCore.Http;
using Web.Configurations.Auth;

namespace FunctionalTests.Security;

public class CurrentUserExceptionMiddlewareTests
{
    [Fact]
    public async Task WhenNextThrowsCurrentUserResolutionException_Returns503()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new CurrentUserExceptionMiddleware(
            _ => throw new CurrentUserResolutionException("Failed to resolve current user."));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status503ServiceUnavailable);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        body.ShouldBe("Service temporarily unavailable.");
    }

    [Fact]
    public async Task WhenNextThrowsCurrentUserUnauthorizedException_Returns401()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new CurrentUserExceptionMiddleware(
            _ => throw new CurrentUserUnauthorizedException("Current user is not authorized."));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        body.ShouldBe("Unauthorized.");
    }

    [Fact]
    public async Task WhenNextSucceeds_CallsThroughAndDoesNotChangeStatusCode()
    {
        var context = new DefaultHttpContext();
        var nextCalled = false;

        var middleware = new CurrentUserExceptionMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        nextCalled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }
}
