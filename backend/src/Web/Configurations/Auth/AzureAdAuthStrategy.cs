using Microsoft.Identity.Web;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using Scalar.AspNetCore;

namespace Web.Configurations.Auth;

/// <summary>
/// Auth strategy backed by Azure AD / Microsoft Entra ID.
/// Requires "AzureAd" configuration section to be populated.
/// </summary>
public class AzureAdAuthStrategy : IAuthStrategy
{
    public void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");
        services.AddAuthorization();
    }

    public void ConfigureMiddleware(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public void ConfigureSwaggerAuth(AspNetCoreOpenApiDocumentGeneratorSettings settings, IConfiguration configuration)
    {
        var tenantId = configuration["AzureAd:TenantId"] ?? "common";
        var clientId = configuration["AzureAd:ClientId"] ?? string.Empty;
        var baseUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0";

        settings.AddAuth("oauth2", new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.OAuth2,
            Flows = new NSwag.OpenApiOAuthFlows
            {
                AuthorizationCode = new NSwag.OpenApiOAuthFlow
                {
                    AuthorizationUrl = $"{baseUrl}/authorize",
                    TokenUrl = $"{baseUrl}/token",
                    Scopes = new Dictionary<string, string>
                    {
                        {
                            configuration["AzureAd:Scopes"] ?? $"{clientId}/.default", "Access API as user"
                        }
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
            flow.WithClientId(configuration["AzureAd:ClientId"] ?? string.Empty)
                .WithSelectedScopes([configuration["AzureAd:DefaultScope"] ?? string.Empty])
                .WithPkce(Pkce.Sha256);
        });
    }
}
