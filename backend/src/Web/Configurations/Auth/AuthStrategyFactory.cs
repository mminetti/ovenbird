using Microsoft.AspNetCore.Authentication;

namespace Web.Configurations.Auth;

public static class AuthStrategyFactory
{
    private const string ConfigKey = "Authentication:Provider";

    public static IAuthStrategy Create(IConfiguration configuration)
    {
        var provider = configuration[ConfigKey] ?? string.Empty;

        return provider.Trim().ToLowerInvariant() switch
        {
            "azuread" => new AzureAdAuthStrategy(),
            "none" or "" => new NoAuthStrategy(),
            _ => throw new InvalidOperationException($"Unknown authentication provider '{provider}'.")
        };
    }

    /// <summary>
    /// Registers the resolved <see cref="IAuthStrategy"/> as a singleton and configures its services.
    /// Returns the strategy so callers can use it before the DI container is built (e.g., Swagger setup).
    /// </summary>
    public static IAuthStrategy AddAuthStrategy(this IServiceCollection services, IConfiguration configuration)
    {
        var strategy = Create(configuration);
        services.AddSingleton(strategy);
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
        services.AddScoped<IClaimsTransformation, CurrentUserClaimsTransformation>();
        strategy.ConfigureServices(services, configuration);
        return strategy;
    }
}
