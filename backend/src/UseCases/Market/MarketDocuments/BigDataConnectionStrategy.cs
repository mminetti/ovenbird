using Core.Shared;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments;

public class BigDataConnectionStrategy(ISftpService sftpService) : IMarketConnectionStrategy
{
    public const string Identifier = "BigData";

    private const string SftpIntegrationIdentifier = "Sftp";
    private const string HostFieldIdentifier = "Host";
    private const string PortFieldIdentifier = "Port";
    private const string UsernameFieldIdentifier = "Username";
    private const string PasswordFieldIdentifier = "Password";

    public string MarketIdentifier => Identifier;

    public Task<IReadOnlyList<string>> ListFilesAsync(Company company, string remoteDirectory, CancellationToken ct)
    {
        return sftpService.ListAsync(ResolveSftpOptions(company), remoteDirectory, ct);
    }

    public Task<Stream> DownloadFileAsync(Company company, string remoteFilePath, CancellationToken ct)
    {
        return sftpService.DownloadAsync(ResolveSftpOptions(company), remoteFilePath, ct);
    }

    public Task UploadFileAsync(Company company, Stream content, string remoteFilePath, CancellationToken ct)
    {
        return sftpService.UploadAsync(ResolveSftpOptions(company), content, remoteFilePath, ct);
    }

    private static SftpOptions ResolveSftpOptions(Company company)
    {
        var integration = company.SystemIntegrations.FirstOrDefault(si =>
            string.Equals(si.Identifier, SftpIntegrationIdentifier, StringComparison.OrdinalIgnoreCase));

        if (integration is null)
        {
            throw new InvalidOperationException(
                $"Company '{company.Name}' (Id: {company.Id}) has no '{SftpIntegrationIdentifier}' system integration configured.");
        }

        return new SftpOptions
        {
            Host = GetRequiredFieldValue(company, integration, HostFieldIdentifier),
            Port = int.TryParse(GetFieldValue(integration, PortFieldIdentifier), out var port) ? port : 22,
            Username = GetRequiredFieldValue(company, integration, UsernameFieldIdentifier),
            Password = GetRequiredFieldValue(company, integration, PasswordFieldIdentifier)
        };
    }

    private static string? GetFieldValue(SystemIntegration integration, string fieldIdentifier)
    {
        return integration.SystemIntegrationFields
            .FirstOrDefault(f => string.Equals(f.Identifier, fieldIdentifier, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    private static string GetRequiredFieldValue(Company company, SystemIntegration integration, string fieldIdentifier)
    {
        var value = GetFieldValue(integration, fieldIdentifier);

        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException(
                $"Company '{company.Name}' (Id: {company.Id}) is missing required '{fieldIdentifier}' field on its '{SftpIntegrationIdentifier}' system integration.");
        }

        return value;
    }
}
