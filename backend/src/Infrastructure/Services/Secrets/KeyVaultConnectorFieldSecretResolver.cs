using Azure;
using Azure.Security.KeyVault.Secrets;
using Core.Shared;
using UseCases.Interfaces.Secrets;

namespace Infrastructure.Services.Secrets;

public class KeyVaultConnectorFieldSecretResolver(SecretClient? secretClient) : IConnectorFieldSecretResolver
{
    public async Task<string> ResolveAsync(ConnectorField field, CancellationToken ct)
    {
        if (!field.IsSecret)
        {
            return field.Value ?? throw new InvalidOperationException(
                $"Connector field '{field.Name}' has no value.");
        }

        var secretName = field.Value;

        if (string.IsNullOrWhiteSpace(secretName))
        {
            throw new InvalidOperationException(
                $"Connector field '{field.Name}' is marked secret but has no reference key.");
        }

        if (secretClient is not null)
        {
            try
            {
                var secret = await secretClient.GetSecretAsync(secretName, cancellationToken: ct);

                return secret.Value.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Not found in Key Vault - fall back to environment variable below.
            }
        }

        var envValue = Environment.GetEnvironmentVariable(secretName);

        if (!string.IsNullOrWhiteSpace(envValue))
        {
            return envValue;
        }

        throw new InvalidOperationException(
            $"Secret '{secretName}' for connector field '{field.Name}' was not found in Key Vault or as an environment variable.");
    }
}
