using Azure;
using Azure.Security.KeyVault.Secrets;
using Core.Shared;
using Infrastructure.Services.Secrets;

namespace UnitTests.Infrastructure.Services.Secrets;

public class KeyVaultConnectorFieldSecretResolverTests : IDisposable
{
    private const string EnvVarName = "KEYVAULT_RESOLVER_TEST_SECRET";

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(EnvVarName, null);
    }

    [Fact]
    public async Task ResolveAsyncReturnsValueUnchangedWhenNotSecret()
    {
        var secretClient = Substitute.For<SecretClient>();
        var resolver = new KeyVaultConnectorFieldSecretResolver(secretClient);
        var field = new ConnectorField { ConnectorId = 1, Name = "host", Value = "sftp.example.com", IsSecret = false };

        var value = await resolver.ResolveAsync(field, CancellationToken.None);

        value.ShouldBe("sftp.example.com");
        await secretClient.DidNotReceive().GetSecretAsync(
            Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<SecretContentType?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveAsyncReturnsValueFromKeyVaultWhenFound()
    {
        var secretClient = Substitute.For<SecretClient>();
        var secretProperties = SecretModelFactory.SecretProperties(new Uri("https://vault.example.com/secrets/prod-ftp-password"));
        var secret = SecretModelFactory.KeyVaultSecret(secretProperties, "vault-value");
        var response = Response.FromValue(secret, Substitute.For<Response>());

        secretClient
            .GetSecretAsync("prod-ftp-password", Arg.Any<string?>(), Arg.Any<SecretContentType?>(), Arg.Any<CancellationToken>())
            .Returns(response);

        var resolver = new KeyVaultConnectorFieldSecretResolver(secretClient);
        var field = new ConnectorField { ConnectorId = 1, Name = "password", Value = "prod-ftp-password", IsSecret = true };

        var value = await resolver.ResolveAsync(field, CancellationToken.None);

        value.ShouldBe("vault-value");
    }

    [Fact]
    public async Task ResolveAsyncFallsBackToEnvironmentVariableWhenNotFoundInKeyVault()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "env-value");

        var secretClient = Substitute.For<SecretClient>();

        secretClient
            .GetSecretAsync(EnvVarName, Arg.Any<string?>(), Arg.Any<SecretContentType?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Response<KeyVaultSecret>>(new RequestFailedException(404, "not found")));

        var resolver = new KeyVaultConnectorFieldSecretResolver(secretClient);
        var field = new ConnectorField { ConnectorId = 1, Name = "password", Value = EnvVarName, IsSecret = true };

        var value = await resolver.ResolveAsync(field, CancellationToken.None);

        value.ShouldBe("env-value");
    }

    [Fact]
    public async Task ResolveAsyncGoesStraightToEnvironmentVariableWhenSecretClientIsNull()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "env-value");

        var resolver = new KeyVaultConnectorFieldSecretResolver(null);
        var field = new ConnectorField { ConnectorId = 1, Name = "password", Value = EnvVarName, IsSecret = true };

        var value = await resolver.ResolveAsync(field, CancellationToken.None);

        value.ShouldBe("env-value");
    }

    [Fact]
    public async Task ResolveAsyncThrowsWhenSecretIsNotFoundAnywhere()
    {
        var secretClient = Substitute.For<SecretClient>();

        secretClient
            .GetSecretAsync("missing-secret", Arg.Any<string?>(), Arg.Any<SecretContentType?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Response<KeyVaultSecret>>(new RequestFailedException(404, "not found")));

        var resolver = new KeyVaultConnectorFieldSecretResolver(secretClient);
        var field = new ConnectorField { ConnectorId = 1, Name = "password", Value = "missing-secret", IsSecret = true };

        await Should.ThrowAsync<InvalidOperationException>(() => resolver.ResolveAsync(field, CancellationToken.None));
    }
}
