using System.Security.Claims;
using NSwag.Generation.AspNetCore;
using Scalar.AspNetCore;

namespace Web.Configurations.Auth;

/// <summary>
/// No-op auth strategy — use for local development or when authentication is not required.
/// </summary>
public class NoAuthStrategy : IAuthStrategy
{
    private const string NotSupportedMessage =
        "NoAuthStrategy does not support claim resolution; no principal should ever be authenticated under this strategy.";

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }
    public void ConfigureMiddleware(WebApplication app) { }
    public void ConfigureSwaggerAuth(AspNetCoreOpenApiDocumentGeneratorSettings settings, IConfiguration configuration) { }
    public void ConfigureScalarAuth(ScalarOptions options, IConfiguration configuration) { }

    public string? GetExternalIdentifier(ClaimsPrincipal principal) => throw new NotSupportedException(NotSupportedMessage);

    public string GetName(ClaimsPrincipal principal) => throw new NotSupportedException(NotSupportedMessage);

    public string GetEmail(ClaimsPrincipal principal) => throw new NotSupportedException(NotSupportedMessage);
}
