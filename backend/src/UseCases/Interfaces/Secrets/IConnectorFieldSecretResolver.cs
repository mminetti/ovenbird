using Core.Shared;

namespace UseCases.Interfaces.Secrets;

public interface IConnectorFieldSecretResolver
{
    Task<string> ResolveAsync(ConnectorField field, CancellationToken ct);
}
