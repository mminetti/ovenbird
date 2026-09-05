using Core.Constants;
using Core.Market;
using Core.Market.Specifications;
using Core.Shared;
using Core.Shared.Specifications;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Strategies;
using UnitTests.UseCases.Market.MarketDocuments.TestDoubles;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class ImportMarketDocumentHandlerHandle
{
    private const string FtpImplementation = "TestFtp";
    private const string FileStorageImplementation = "TestFileStorage";
    private const string RemoteDirectory = "remote/dir";
    private const string RootDirectory = "import-market-documents";

    private readonly IRepository<MarketDocument> _documentRepository = Substitute.For<IRepository<MarketDocument>>();
    private readonly IReadRepository<MarketDocument> _documentReadRepository = Substitute.For<IReadRepository<MarketDocument>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly TestFtpService _ftpService = new();
    private readonly TestFileStorage _fileStorage = new();
    private readonly ImportMarketDocumentHandler _handler;

    public ImportMarketDocumentHandlerHandle()
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IFtpService>(FtpImplementation, _ftpService);
        services.AddKeyedSingleton<IFileStorage>(FileStorageImplementation, _fileStorage);

        var resolver = new MarketImportStrategyResolver([new BigDataImportStrategy(services.BuildServiceProvider())]);

        _handler = new ImportMarketDocumentHandler(
            _documentRepository,
            _documentReadRepository,
            _configurationReadRepository,
            resolver,
            TimeProvider.System);
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

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeNameSpec>(), Arg.Any<CancellationToken>())
            .Returns([configuration]);

        var existingDocument = new MarketDocument { Id = 99, Name = "existingfile.csv", CompanyId = company.Id };

        _documentReadRepository
            .FirstOrDefaultAsync(Arg.Any<MarketDocumentByNameAndCompanySpec>(), Arg.Any<CancellationToken>())
            .Returns(existingDocument, (MarketDocument?)null);

        long nextId = 100;
        MarketDocument? created = null;
        _documentRepository
            .AddAsync(Arg.Do<MarketDocument>(d => created = d), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var document = callInfo.Arg<MarketDocument>();
                document.Id = nextId++;
                return document;
            });

        var result = await _handler.Handle(new ImportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([existingDocument.Id, 100]);

        // The already-imported file must never be re-downloaded or re-uploaded.
        _ftpService.DownloadedPaths.ShouldBe([$"{RemoteDirectory}/newfile.csv"]);
        _fileStorage.UploadedPaths.Count.ShouldBe(1);

        created.ShouldNotBeNull();
        created!.Name.ShouldBe("newfile.csv");
        created.CompanyId.ShouldBe(company.Id);
        created.DirectionId.ShouldBe(MarketDocumentDirections.Inbound);
        created.StatusId.ShouldBe(MarketDocumentStatuses.New);
        created.File.ShouldStartWith($"{RootDirectory}/");

        var fileStorageOptions = new FileStorageOptions { RootDirectory = RootDirectory };
        await using var roundTripStream = await _fileStorage.OpenReadAsync(fileStorageOptions, created.File, CancellationToken.None);
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
            ConnectorType = new ConnectorType { Id = 1, Name = ConnectorTypes.Ftp },
            ConnectorImplementation = new ConnectorImplementation { Id = 1, Name = FtpImplementation },
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
            ConnectorType = new ConnectorType { Id = 2, Name = ConnectorTypes.FileStorage },
            ConnectorImplementation = new ConnectorImplementation { Id = 2, Name = FileStorageImplementation },
            ConnectorFields =
            [
                new ConnectorField { ConnectorId = 2, Name = "root.directory", Value = RootDirectory },
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
                new ConfigurationField { ConfigurationId = 1, Name = "ftp.remote.directory", Value = RemoteDirectory },
            ],
        };
    }
}
