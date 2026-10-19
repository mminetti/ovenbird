using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Infrastructure.Services.Files;
using Infrastructure.Services.Market.BigData;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.UseCases.Market.MarketDocuments.TestDoubles;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.Infrastructure.Services.Market.BigData;

public class BigDataEdiImportHandlerExecuteImportAsync
{
    private const string FtpImplementation = "TestFtp";
    private const string FileStorageImplementation = "TestFileStorage";
    private const string RemoteDirectory = "remote/dir";
    private const string RootDirectory = "import-market-documents";
    private const string FileStorageFolder = "edi/import";

    private readonly IReadRepository<MarketDocument> _documentReadRepository = Substitute.For<IReadRepository<MarketDocument>>();
    private readonly TestFtpService _ftpService = new();
    private readonly TestFileStorage _fileStorage = new();
    private readonly BigDataEdiImportHandler _handler;

    public BigDataEdiImportHandlerExecuteImportAsync()
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IFtpService>(FtpImplementation, _ftpService);
        services.AddKeyedSingleton<IFileStorage>(FileStorageImplementation, _fileStorage);

        var secretResolver = Substitute.For<IConnectorFieldSecretResolver>();
        secretResolver
            .ResolveAsync(Arg.Any<ConnectorField>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(((ConnectorField)ci[0]).Value!));

        var serviceProvider = services.BuildServiceProvider();
        var ftpTransport = new ConnectorFtpTransport(serviceProvider, secretResolver);
        var fileStorageTransport = new ConnectorFileStorageTransport(serviceProvider, secretResolver, TimeProvider.System);

        _handler = new BigDataEdiImportHandler(
            ftpTransport,
            fileStorageTransport,
            _documentReadRepository,
            new MarketDocumentParserResolver([]));
    }

    [Fact]
    public async Task ImportsNewFilesAndSkipsAlreadyImportedFiles_RoundTrippingContentThroughFtpAndFileStorage()
    {
        const string newFileContent = "A,B,C\n1,2,3";
        _ftpService
            .SeedFile($"{RemoteDirectory}/existingfile.csv", "already imported")
            .SeedFile($"{RemoteDirectory}/newfile.csv", newFileContent);

        var company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" };
        var configuration = CreateConfiguration(company);

        var existingDocument = new MarketDocument { Id = 99, Identifier = "existingfile.csv", CompanyId = company.Id };

        _documentReadRepository
            .FirstOrDefaultAsync(Arg.Any<MarketDocumentByIdentifierAndCompanySpec>(), Arg.Any<CancellationToken>())
            .Returns(existingDocument, (MarketDocument?)null);

        var documents = await _handler.ExecuteImportAsync(configuration, CancellationToken.None);

        documents.Count.ShouldBe(2);
        documents[0].ShouldBeSameAs(existingDocument);

        // The already-imported file must never be re-downloaded or re-uploaded.
        _ftpService.DownloadedPaths.ShouldBe([$"{RemoteDirectory}/newfile.csv"]);
        _fileStorage.UploadedPaths.Count.ShouldBe(1);

        var created = documents[1];
        created.Id.ShouldBe(0L);
        created.Identifier.ShouldBe("newfile.csv");
        created.CompanyId.ShouldBe(company.Id);
        created.DirectionId.ShouldBe(Constants.MarketDocumentDirections.Inbound);
        created.StatusId.ShouldBe(Constants.MarketDocumentStatuses.New);
        created.Asset.ShouldNotBeNull();
        created.Asset!.Reference.ShouldStartWith($"{RootDirectory}/");

        var fileStorageOptions = new FileStorageOptions { RootDirectory = RootDirectory };
        await using var roundTripStream = await _fileStorage.OpenReadAsync(fileStorageOptions, created.Asset!.Reference, CancellationToken.None);
        using var reader = new StreamReader(roundTripStream);
        var roundTrippedContent = await reader.ReadToEndAsync(CancellationToken.None);

        roundTrippedContent.ShouldBe(newFileContent);
    }

    private static Configuration CreateConfiguration(Company company)
    {
        var ftpConnector = new Connector
        {
            Id = 1,
            Name = "Ftp",
            ConnectorImplementation = new ConnectorImplementation
            {
                Id = 1,
                Name = "Test FTP",
                Identifier = FtpImplementation,
                ConnectorTypeId = Constants.ConnectorTypes.Ftp,
                ConnectorType = new ConnectorType { Id = Constants.ConnectorTypes.Ftp, Name = "FTP" },
            },
            ConnectorFields =
            [
                new ConnectorField { ConnectorId = 1, Name = "host", Value = "sftp.example.com" },
                new ConnectorField { ConnectorId = 1, Name = "port", Value = "22" },
                new ConnectorField { ConnectorId = 1, Name = "username", Value = "user" },
                new ConnectorField { ConnectorId = 1, Name = "password", Value = "pass" },
            ],
        };

        var fileStorageConnector = new Connector
        {
            Id = 2,
            Name = "FileStorage",
            ConnectorImplementation = new ConnectorImplementation
            {
                Id = 2,
                Name = "Test File Storage",
                Identifier = FileStorageImplementation,
                ConnectorTypeId = Constants.ConnectorTypes.FileStorage,
                ConnectorType = new ConnectorType { Id = Constants.ConnectorTypes.FileStorage, Name = "File Storage" },
            },
            ConnectorFields =
            [
                new ConnectorField { ConnectorId = 2, Name = "connection.string", Value = "UseDevelopmentStorage=true" },
            ],
        };

        return new Configuration
        {
            Id = 1,
            Name = "edi.import",
            Company = company,
            CompanyId = company.Id,
            Connectors = [ftpConnector, fileStorageConnector],
            ConfigurationFields =
            [
                new ConfigurationField { ConfigurationId = 1, Name = "handler", Value = "BigData" },
                new ConfigurationField { ConfigurationId = 1, Name = "ftp.root.directory", Value = RemoteDirectory },
                new ConfigurationField { ConfigurationId = 1, Name = "file.storage.root.directory", Value = RootDirectory },
                new ConfigurationField { ConfigurationId = 1, Name = "file.storage.folder", Value = FileStorageFolder },
            ],
        };
    }
}
