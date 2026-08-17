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
}
