using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Web.Configurations.Auth;

namespace FunctionalTests;

/// <summary>
/// Authentication handler used in tests: authenticates every request as a
/// super-user who already has all application permissions resolved.
/// </summary>
sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "TestScheme";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Test User"),
            new(AuthConstants.CurrentUserResolvedClaimType, "true"),
        };

        foreach (var permission in AllPermissions())
            claims.Add(new Claim(AuthConstants.PermissionsClaimType, permission));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static IEnumerable<string> AllPermissions() =>
    [
        UseCases.Common.Constants.Constants.Permissions.UsersRead,
        UseCases.Common.Constants.Constants.Permissions.UsersWrite,
        UseCases.Common.Constants.Constants.Permissions.RolesRead,
        UseCases.Common.Constants.Constants.Permissions.RolesWrite,
        UseCases.Common.Constants.Constants.Permissions.PermissionsRead,
        UseCases.Common.Constants.Constants.Permissions.PermissionsWrite,
    ];
}
