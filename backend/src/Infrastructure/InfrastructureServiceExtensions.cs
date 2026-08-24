using Core.Security.Interfaces;
using Core.Security.Services;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Data.Queries.Security;
using Infrastructure.Services.Files;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments;
using UseCases.Security.Modules.List;
using UseCases.Security.Permissions.List;
using UseCases.Security.Roles.List;
using UseCases.Security.Users;
using UseCases.Security.Users.List;

namespace Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
      this IServiceCollection services,
      ConfigurationManager config,
      ILogger logger)
    {
        string? connectionString = config.GetConnectionString("db") ?? config.GetConnectionString("DefaultConnection");

        string? readConnectionString = config.GetConnectionString("db") ?? config.GetConnectionString("ReadConnection");

        Guard.Against.Null(connectionString);

        services.AddHybridCache();
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<EventDispatchInterceptor>();
        services.AddScoped<IDomainEventDispatcher, WolverineDomainEventDispatcher>();

        // Register write DbContext
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();

            options.UseSqlServer(connectionString);
            options.AddInterceptors(eventDispatchInterceptor);
        });

        // Register read DbContext
        services.AddDbContext<ReadDbContext>(options =>
        {
            options.UseSqlServer(readConnectionString);
        });

        services.Configure<FtpOptions>(config.GetSection(FtpOptions.SectionName));
        services.Configure<AzureBlobStorageOptions>(config.GetSection(AzureBlobStorageOptions.SectionName));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>))
            .AddScoped<IListUsersQueryService, ListUsersQueryService>()
            .AddScoped<IListModulesQueryService, ListModulesQueryService>()
            .AddScoped<IListRolesQueryService, ListRolesQueryService>()
            .AddScoped<IListPermissionsQueryService, ListPermissionsQueryService>()
            .AddScoped<IDeleteUserService, DeleteUserService>()
            .AddScoped<IFtpService, FtpService>()
            .AddScoped<IFileStorage, AzureBlobFileStorage>()
            .AddScoped<IMarketDocumentStrategy, BigDataMarketDocumentStrategy>()
            .AddScoped<MarketDocumentStrategyResolver>()
            .AddScoped<CurrentUserService>();

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}
