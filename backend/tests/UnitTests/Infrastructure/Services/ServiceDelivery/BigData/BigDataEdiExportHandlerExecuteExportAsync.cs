using System.Text;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Infrastructure.Services.Files;
using Infrastructure.Services.ServiceDelivery.BigData;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.UseCases.Market.MarketDocuments.TestDoubles;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData;

public class BigDataEdiExportHandlerExecuteExportAsync
{
    private const string FtpImplementation = "TestFtp";
    private const string FileStorageImplementation = "TestFileStorage";
    private const string RemoteDirectory = "remote/export";
    private const string RootDirectory = "export-market-documents";
    private const string FileStorageFolder = "edi/export";

    private readonly IReadRepository<MarketDocumentItem> _itemReadRepository = Substitute.For<IReadRepository<MarketDocumentItem>>();
    private readonly TestFtpService _ftpService = new();
    private readonly TestFileStorage _fileStorage = new();
    private readonly IMarketDocumentGenerator _generator = Substitute.For<IMarketDocumentGenerator>();
    private readonly BigDataEdiExportHandler _handler;

    public BigDataEdiExportHandlerExecuteExportAsync()
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

        _generator.Identifier.Returns("BigData");

        _handler = new BigDataEdiExportHandler(
            ftpTransport,
            fileStorageTransport,
            _itemReadRepository,
            new MarketDocumentGeneratorResolver([_generator]),
            TimeProvider.System);
    }

    [Fact]
    public async Task CombinesPendingItemsIntoOneFile_UploadsToStorageAndFtp_AndReturnsTheDocumentAndItems()
    {
        var pendingItems = new List<MarketDocumentItem>
        {
            new() { Id = 1, Set = "814", SubSet = "01", Raw = "<Transaction><TransactionSet>814</TransactionSet></Transaction>" },
            new() { Id = 2, Set = "814", SubSet = "16", Raw = "<Transaction><TransactionSet>814</TransactionSet></Transaction>" },
        };

        SeedPendingItems(pendingItems);

        const string generatedContent = "<Document><TransactionList/></Document>";
        SeedGeneratedContent(generatedContent);

        var company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" };
        var configuration = CreateConfiguration(company);

        var result = await _handler.ExecuteExportAsync(configuration, CancellationToken.None);

        result.ShouldNotBeNull();
        result!.Items.ShouldBe(pendingItems);

        result.Document.CompanyId.ShouldBe(company.Id);
        result.Document.DirectionId.ShouldBe(Constants.MarketDocumentDirections.Outbound);
        result.Document.StatusId.ShouldBe(Constants.MarketDocumentStatuses.New);
        result.Document.Asset.ShouldNotBeNull();
        result.Document.Asset!.Reference.ShouldStartWith($"{RootDirectory}/");

        _fileStorage.UploadedPaths.Count.ShouldBe(1);
        _ftpService.UploadedPaths.Count.ShouldBe(1);
        _ftpService.UploadedPaths[0].ShouldStartWith($"{RemoteDirectory}/");

        var uploadedFtpContent = _ftpService.GetUploadedContent(_ftpService.UploadedPaths[0]);
        uploadedFtpContent.ShouldNotBeNull();
        Encoding.UTF8.GetString(uploadedFtpContent!).ShouldBe(generatedContent);

        var fileStorageOptions = new FileStorageOptions { RootDirectory = RootDirectory };
        await using var roundTripStream = await _fileStorage.OpenReadAsync(fileStorageOptions, result.Document.Asset!.Reference, CancellationToken.None);
        using var reader = new StreamReader(roundTripStream);
        var roundTrippedContent = await reader.ReadToEndAsync(CancellationToken.None);

        roundTrippedContent.ShouldBe(generatedContent);
    }

    [Fact]
    public async Task ReturnsNullWhenThereAreNoPendingItems()
    {
        SeedPendingItems([]);

        var configuration = CreateConfiguration(new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" });

        var result = await _handler.ExecuteExportAsync(configuration, CancellationToken.None);

        result.ShouldBeNull();
        _fileStorage.UploadedPaths.ShouldBeEmpty();
        _ftpService.UploadedPaths.ShouldBeEmpty();
    }

    [Fact]
    public async Task SecondCallDoesNotReturnItemsAlreadyAssignedToADocument()
    {
        var pendingItems = new List<MarketDocumentItem>
        {
            new() { Id = 1, Set = "814", SubSet = "01", Raw = "<Transaction/>" },
        };

        SeedPendingItems(pendingItems);
        SeedGeneratedContent("<Document/>");

        var configuration = CreateConfiguration(new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" });

        var firstResult = await _handler.ExecuteExportAsync(configuration, CancellationToken.None);
        firstResult.ShouldNotBeNull();

        // Simulate the orchestrator persisting the document and stamping MarketDocumentId on the item.
        pendingItems[0].MarketDocumentId = 700;

        var secondResult = await _handler.ExecuteExportAsync(configuration, CancellationToken.None);

        secondResult.ShouldBeNull();
    }

    private void SeedPendingItems(List<MarketDocumentItem> pendingItems)
    {
        _itemReadRepository
            .ListAsync(Arg.Any<MarketDocumentItemsPendingExportSpec>(), Arg.Any<CancellationToken>())
            .Returns(_ => pendingItems.Where(i => i.MarketDocumentId == null).ToList());
    }

    private void SeedGeneratedContent(string content)
    {
        _generator
            .WriteTransactionsAsync(Arg.Any<IReadOnlyList<MarketDocumentItem>>(), Arg.Any<CancellationToken>())
            .Returns(_ => new MemoryStream(Encoding.UTF8.GetBytes(content)));
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
            Name = "edi.export",
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
