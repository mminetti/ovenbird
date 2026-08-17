using Core.Interfaces;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Data.Queries;
using UseCases.Contributors.List;

namespace Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
      this IServiceCollection services,
      ConfigurationManager config,
      ILogger logger)
    {
        string? connectionString = config.GetConnectionString("db")
            ?? config.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString);

        services.AddScoped<EventDispatchInterceptor>();
        services.AddScoped<IDomainEventDispatcher, WolverineDomainEventDispatcher>();

        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();

            options.UseSqlServer(connectionString);
            options.AddInterceptors(eventDispatchInterceptor);
        });

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
               .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
               .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
               .AddScoped<IDeleteContributorService, DeleteContributorService>();

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}
