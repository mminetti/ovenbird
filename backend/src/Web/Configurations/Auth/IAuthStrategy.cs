using NSwag.Generation.AspNetCore;
using Scalar.AspNetCore;

namespace Web.Configurations.Auth;

public static class AuthConstants
{
    /// <summary>The claim type used to carry permission strings for the current user.</summary>
    public const string PermissionsClaimType = "permissions";

    /// <summary>Marker claim type added once current-user resolution has run, to guard against
    /// re-running it if claims transformation is invoked more than once per request.</summary>
    public const string CurrentUserResolvedClaimType = "current-user-resolved";
}

/// <summary>
/// Encapsulates auth setup for a given provider across the three ASP.NET Core pipeline stages:
/// service registration, middleware, and API doc tooling (Swagger/Scalar).
/// </summary>
public interface IAuthStrategy
{
    /// <summary>Registers authentication and authorization services.</summary>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>Adds authentication middleware to the request pipeline.</summary>
    void ConfigureMiddleware(WebApplication app);

    /// <summary>Configures security scheme and scopes in the OpenAPI/Swagger document.</summary>
    void ConfigureSwaggerAuth(AspNetCoreOpenApiDocumentGeneratorSettings settings, IConfiguration configuration);

    /// <summary>Configures auth flow in the Scalar API reference UI.</summary>
    void ConfigureScalarAuth(ScalarOptions options, IConfiguration configuration);
}
