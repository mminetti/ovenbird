using Core.ContributorAggregate;
using Infrastructure;
using Infrastructure.Data;
using UseCases.Contributors.Create;

namespace Web.Configurations;

public static class MediatorConfig
{
    // Should be called from ServiceConfigs.cs, not Program.cs
    public static WebApplicationBuilder AddWolverine(this WebApplicationBuilder builder,
      Microsoft.Extensions.Logging.ILogger logger)
    {
        logger.LogInformation("Registering Wolverine");

        builder.Host.UseWolverine(opts =>
        {
            // Supply any TYPE from each assembly you want scanned
            opts.Discovery.IncludeAssembly(typeof(Contributor).Assembly);          // Core
            opts.Discovery.IncludeAssembly(typeof(CreateContributorCommand).Assembly); // UseCases
            opts.Discovery.IncludeAssembly(typeof(InfrastructureServiceExtensions).Assembly); // Infrastructure
            opts.Discovery.IncludeAssembly(typeof(MediatorConfig).Assembly);       // Web

            // EF Core always registers DbContextOptions<T> via an internal opaque lambda factory
            // (EntityFrameworkServiceCollectionExtensions.CreateDbContextOptions).
            // This is fundamental to EF Core's DI integration and cannot be avoided.
            // We tell Wolverine to use service location specifically for AppDbContext.
            opts.CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>();
        });

        return builder;
    }
}
