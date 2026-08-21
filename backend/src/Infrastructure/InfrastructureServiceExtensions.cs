using Core.Interfaces.Security;
using Core.Services.Security;
using Infrastructure.Data;
using Infrastructure.Data.Queries.Security;
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

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>))
            .AddScoped<IListUsersQueryService, ListUsersQueryService>()
            .AddScoped<IListModulesQueryService, ListModulesQueryService>()
            .AddScoped<IListRolesQueryService, ListRolesQueryService>()
            .AddScoped<IListPermissionsQueryService, ListPermissionsQueryService>()
            .AddScoped<IDeleteUserService, DeleteUserService>()
            .AddScoped<CurrentUserService>();

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}
