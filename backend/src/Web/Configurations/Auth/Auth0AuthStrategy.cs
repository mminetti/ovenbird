using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using Scalar.AspNetCore;

namespace Web.Configurations.Auth;

/// <summary>
/// Auth strategy backed by Auth0, using standard OIDC/JWT bearer validation.
/// Requires the "Auth0" configuration section to be populated (Domain, Audience,
/// and optionally ClientId/ClaimsNamespace for interactive Swagger/Scalar login).
/// See docs/auth0-setup.md for the manual Auth0 dashboard prerequisites.
/// </summary>
public class Auth0AuthStrategy(IConfiguration configuration) : IAuthStrategy
{
    private readonly string _claimsNamespace = configuration["Auth0:ClaimsNamespace"] ?? "https://ovenbird.example.com/";

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var domain = configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0:Domain is required.");
        var audience = configuration["Auth0:Audience"] ?? throw new InvalidOperationException("Auth0:Audience is required.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{domain}/";
                options.Audience = audience;

                // Keep claim types exactly as they appear in the Auth0-issued JWT (e.g. "sub")
                // instead of the default inbound remapping to legacy long-form ClaimTypes.* URIs.
                options.MapInboundClaims = false;
            });
    }

    public void ConfigureMiddleware(WebApplication app)
    {
        app.UseAuthentication();
    }

    public void ConfigureSwaggerAuth(AspNetCoreOpenApiDocumentGeneratorSettings settings, IConfiguration configuration)
    {
        var domain = configuration["Auth0:Domain"] ?? string.Empty;
        var audience = configuration["Auth0:Audience"] ?? string.Empty;
        var baseUrl = $"https://{domain}";

        settings.AddAuth("oauth2", new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.OAuth2,
            Flows = new NSwag.OpenApiOAuthFlows
            {
                AuthorizationCode = new NSwag.OpenApiOAuthFlow
                {
                    AuthorizationUrl = $"{baseUrl}/authorize",
                    TokenUrl = $"{baseUrl}/oauth/token",
                    Scopes = new Dictionary<string, string>
                    {
                        { audience, "Access API as user" }
                    }
                }
            }
        });

        settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("oauth2"));
    }

    public void ConfigureScalarAuth(ScalarOptions options, IConfiguration configuration)
    {
        options.AddAuthorizationCodeFlow("oauth2", flow =>
        {
            flow.WithClientId(configuration["Auth0:ClientId"] ?? string.Empty)
                .WithSelectedScopes([configuration["Auth0:Audience"] ?? string.Empty])
                .WithPkce(Pkce.Sha256);
        });
    }

    public string? GetExternalIdentifier(ClaimsPrincipal principal) =>
        principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    public string GetName(ClaimsPrincipal principal) =>
        principal.FindFirst($"{_claimsNamespace}name")?.Value ?? string.Empty;

    public string GetEmail(ClaimsPrincipal principal) =>
        principal.FindFirst($"{_claimsNamespace}email")?.Value ?? string.Empty;
}
