using NSwag.Generation.AspNetCore;
using Scalar.AspNetCore;

namespace Web.Configurations.Auth;

/// <summary>
/// No-op auth strategy — use for local development or when authentication is not required.
/// </summary>
public class NoAuthStrategy : IAuthStrategy
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }
    public void ConfigureMiddleware(WebApplication app) { }
    public void ConfigureSwaggerAuth(AspNetCoreOpenApiDocumentGeneratorSettings settings, IConfiguration configuration) { }
    public void ConfigureScalarAuth(ScalarOptions options, IConfiguration configuration) { }
}
