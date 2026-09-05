using Core.Shared;

namespace UseCases.Interfaces.Secrets;

public static class ConnectorSecretResolutionExtensions
{
    public static Task<string> GetRequiredResolvedValueAsync(
        this Connector connector, string name, IConnectorFieldSecretResolver resolver, CancellationToken ct)
    {
        var field = connector.ConnectorFields
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

        if (field is null || string.IsNullOrWhiteSpace(field.Value))
        {
            throw new InvalidOperationException($"Connector '{connector.Name}' is missing required '{name}' field.");
        }

        return resolver.ResolveAsync(field, ct);
    }
}
