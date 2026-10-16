namespace Infrastructure.Services.Files;

internal static class KeyedServiceResolver
{
    public static TService Resolve<TService>(IServiceProvider serviceProvider, string configurationName, string implementation)
        where TService : notnull
    {
        try
        {
            return serviceProvider.GetRequiredKeyedService<TService>(implementation);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"Configuration '{configurationName}' implementation '{implementation}' is not registered.", ex);
        }
    }
}
