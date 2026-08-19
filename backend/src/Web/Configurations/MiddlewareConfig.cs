using Ardalis.ListStartupServices;
using Infrastructure.Data;
using Scalar.AspNetCore;
using Web.Configurations.Auth;

namespace Web.Configurations;

public static class MiddlewareConfig
{
    public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
        }
        else
        {
            app.UseHsts();
        }

        app.UseDefaultExceptionHandler(); // from FastEndpoints

        var authStrategy = AuthStrategyFactory.Create(app.Configuration);

        authStrategy.ConfigureMiddleware(app);

        app.UseFastEndpoints();

        app.UseSwaggerGen(options =>
        {
            options.Path = "/openapi/{documentName}.json";
        },
        settings =>
        {
            settings.Path = "/swagger";
            settings.DocumentPath = "/openapi/{documentName}.json";
        });

        

        app.MapScalarApiReference("/docs", options =>
        {
            options.WithTitle("Ovenbird API");
            options.SortTagsAlphabetically();
            options.SortOperationsByMethod();

            options.WithOpenApiRoutePattern("/openapi/{documentName}.json");

            authStrategy.ConfigureScalarAuth(options, app.Configuration);
        });

        app.UseHttpsRedirection(); // Note this will drop Authorization headers

        // Run migrations and seed when explicitly requested via environment variable
        var shouldMigrate = app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

        if (shouldMigrate)
        {
            await MigrateDatabaseAsync(app);
            await SeedDatabaseAsync(app);
        }

        return app;
    }

    static async Task MigrateDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();

            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred migrating the DB. {exceptionMessage}", ex.Message);
            throw; // Re-throw to make startup fail if migrations fail
        }
    }

    static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Seeding database...");

            var context = services.GetRequiredService<AppDbContext>();
            await SeedData.InitializeAsync(context);

            logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
            // Don't re-throw for seeding errors - it's not critical
        }
    }
}
