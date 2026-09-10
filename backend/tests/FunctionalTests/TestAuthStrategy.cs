using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NSwag.Generation.AspNetCore;
using Scalar.AspNetCore;
using Web.Configurations.Auth;

namespace FunctionalTests;

/// <summary>
/// Auth strategy used during functional tests: sets up <see cref="TestAuthHandler"/>
/// so every request is authenticated with a fully-privileged principal.
/// </summary>
sealed class TestAuthStrategy : IAuthStrategy
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
    }

    public void ConfigureMiddleware(WebApplication app) => app.UseAuthentication();

    public void ConfigureSwaggerAuth(AspNetCoreOpenApiDocumentGeneratorSettings settings, IConfiguration configuration) { }
    public void ConfigureScalarAuth(ScalarOptions options, IConfiguration configuration) { }

    public string? GetExternalIdentifier(ClaimsPrincipal principal) => principal.Identity?.Name;
    public string GetName(ClaimsPrincipal principal) => principal.Identity?.Name ?? "Test User";
    public string GetEmail(ClaimsPrincipal principal) => "test-user@example.com";
}
