using NSwag.Generation.AspNetCore;
using Scalar.AspNetCore;

namespace Web.Configurations.Auth;

public interface IAuthStrategy
{
    void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder);
    void ConfigureMiddleware(WebApplication app);
    void ConfigureSwaggerAuth(AspNetCoreOpenApiDocumentGeneratorSettings settings, IConfiguration configuration);
    void ConfigureScalarAuth(ScalarOptions options, IConfiguration configuration);
}
