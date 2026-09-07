using Core.Shared;

namespace UseCases.Interfaces.Secrets;

public static class ConnectorSecretResolutionExtensions
{
    public static Task<string> GetRequiredValueAsync(
        this Connector connector, string name, IConnectorFieldSecretResolver resolver, CancellationToken ct)
    {
        var field = connector.GetField(name);

        if (field is null || string.IsNullOrWhiteSpace(field.Value))
        {
            throw new InvalidOperationException($"Connector '{connector.Name}' is missing required '{name}' field.");
        }

        return field.IsSecret ? resolver.ResolveAsync(field, ct) : Task.FromResult(field.Value);
    }
}
