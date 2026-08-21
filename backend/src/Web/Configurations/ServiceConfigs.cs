using Infrastructure;

namespace Web.Configurations;

public static class ServiceConfigs
{
    public static IServiceCollection AddServiceConfigs(
        this IServiceCollection services,
        Microsoft.Extensions.Logging.ILogger logger, 
        WebApplicationBuilder builder)
    {
        services.AddInfrastructureServices(builder.Configuration, logger);
        builder.AddWolverine(logger);

        logger.LogInformation("{Project} services registered", "Wolverine and Email Sender");

        return services;
    }
}
