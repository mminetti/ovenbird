using System.Security.Claims;
using Ardalis.Result;
using UseCases.Security.Users;
using UseCases.Security.Users.GetOrCreate;
using Web.Configurations.Auth;
using Wolverine;

namespace FunctionalTests.Security;

public class CurrentUserClaimsTransformationTests
{
    private const string TestOid = "oid-test-123";

    private static ClaimsPrincipal UnauthenticatedPrincipal()
    {
        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    private static ClaimsPrincipal AuthenticatedPrincipal(string oid = TestOid, params Claim[] extraClaims)
    {
        Claim[] claims =
        [
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", oid),
            .. extraClaims
        ];

        var identity = new ClaimsIdentity(claims, "Bearer");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task WhenUnauthenticated_ReturnsPrincipalUnchangedAndDoesNotCallBus()
    {
        var bus = Substitute.For<IMessageBus>();
        var transformation = new CurrentUserClaimsTransformation(bus);
        var principal = UnauthenticatedPrincipal();

        var result = await transformation.TransformAsync(principal);

        result.ShouldBeSameAs(principal);
        await bus.DidNotReceive().InvokeAsync<Result<CurrentUserInfo>>(
            Arg.Any<GetOrCreateCurrentUserCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WhenAuthenticated_AddsPermissionAndMarkerClaims()
    {
        var userInfo = new CurrentUserInfo(7, TestOid, "Test User", "test@example.com", true)
        {
            Permissions = [new UserPermissionDto("Users:Read", "Security"), new UserPermissionDto("Modules:Read", "Security")]
        };
        var bus = Substitute.For<IMessageBus>();
        bus.InvokeAsync<Result<CurrentUserInfo>>(Arg.Any<GetOrCreateCurrentUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userInfo));

        var transformation = new CurrentUserClaimsTransformation(bus);
        var principal = AuthenticatedPrincipal();

        var result = await transformation.TransformAsync(principal);

        var permClaims = result.FindAll(AuthConstants.PermissionsClaimType).Select(c => c.Value).ToList();
        permClaims.ShouldContain("Users:Read");
        permClaims.ShouldContain("Modules:Read");
        result.HasClaim(c => c.Type == AuthConstants.CurrentUserResolvedClaimType).ShouldBeTrue();
    }

    [Fact]
    public async Task WhenCommandReturnsError_ThrowsCurrentUserResolutionException()
    {
        var bus = Substitute.For<IMessageBus>();
        bus.InvokeAsync<Result<CurrentUserInfo>>(Arg.Any<GetOrCreateCurrentUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Failed to resolve user: connection refused"));

        var transformation = new CurrentUserClaimsTransformation(bus);
        var principal = AuthenticatedPrincipal();

        await Should.ThrowAsync<CurrentUserResolutionException>(() => transformation.TransformAsync(principal));
    }

    [Fact]
    public async Task WhenCommandReturnsUnauthorized_ThrowsCurrentUserUnauthorizedException()
    {
        var bus = Substitute.For<IMessageBus>();
        bus.InvokeAsync<Result<CurrentUserInfo>>(Arg.Any<GetOrCreateCurrentUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Unauthorized());

        var transformation = new CurrentUserClaimsTransformation(bus);
        var principal = AuthenticatedPrincipal();

        await Should.ThrowAsync<CurrentUserUnauthorizedException>(() => transformation.TransformAsync(principal));
    }

    [Fact]
    public async Task WhenAlreadyResolved_SkipsResolutionAndDoesNotCallBus()
    {
        var bus = Substitute.For<IMessageBus>();
        var transformation = new CurrentUserClaimsTransformation(bus);
        var principal = AuthenticatedPrincipal(extraClaims: [new Claim(AuthConstants.CurrentUserResolvedClaimType, "true")]);

        var result = await transformation.TransformAsync(principal);

        result.ShouldBeSameAs(principal);
        await bus.DidNotReceive().InvokeAsync<Result<CurrentUserInfo>>(
            Arg.Any<GetOrCreateCurrentUserCommand>(), Arg.Any<CancellationToken>());
    }
}
