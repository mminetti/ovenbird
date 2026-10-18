using System.Text;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using Infrastructure.Services.Files;
using Infrastructure.Services.Market.BigData;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.UseCases.Market.MarketDocuments.TestDoubles;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using UseCases.Market.MarketDocuments.Export;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class ExportMarketDocumentHandlerHandle
{
    private const string FtpImplementation = "TestFtp";
    private const string FileStorageImplementation = "TestFileStorage";
    private const string RemoteDirectory = "remote/export";
    private const string RootDirectory = "export-market-documents";
    private const string FileStorageFolder = "edi/export";

    private readonly IRepository<MarketDocument> _documentRepository = Substitute.For<IRepository<MarketDocument>>();
    private readonly IRepository<MarketDocumentItem> _itemRepository = Substitute.For<IRepository<MarketDocumentItem>>();
    private readonly IReadRepository<MarketDocumentItem> _itemReadRepository = Substitute.For<IReadRepository<MarketDocumentItem>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly TestFtpService _ftpService = new();
    private readonly TestFileStorage _fileStorage = new();
    private readonly IMarketDocumentGenerator _generator = Substitute.For<IMarketDocumentGenerator>();
    private readonly ExportMarketDocumentHandler _handler;

    public ExportMarketDocumentHandlerHandle()
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
        var ediResolver = new EdiExportHandlerResolver([new BigDataEdiExportHandler(ftpTransport, fileStorageTransport)]);

        _generator.Identifier.Returns("BigData");

        _handler = new ExportMarketDocumentHandler(
            _documentRepository,
            _itemRepository,
            _itemReadRepository,
            _configurationReadRepository,
            ediResolver,
            new MarketDocumentGeneratorResolver([_generator]),
            TimeProvider.System);
    }

    [Fact]
    public async Task CombinesPendingItemsIntoOneFile_UploadsToStorageAndFtp_AndBackfillsMarketDocumentId()
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

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([configuration]);

        MarketDocument? created = null;
        _documentRepository
            .AddAsync(Arg.Do<MarketDocument>(d => created = d), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var document = callInfo.Arg<MarketDocument>();
                document.Id = 500;
                return document;
            });

        var result = await _handler.Handle(new ExportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([500]);

        created.ShouldNotBeNull();
        created!.CompanyId.ShouldBe(company.Id);
        created.DirectionId.ShouldBe(Constants.MarketDocumentDirections.Outbound);
        created.StatusId.ShouldBe(Constants.MarketDocumentStatuses.New);
        created.Asset.ShouldNotBeNull();
        created.Asset!.Reference.ShouldStartWith($"{RootDirectory}/");

        pendingItems[0].MarketDocumentId.ShouldBe(500L);
        pendingItems[1].MarketDocumentId.ShouldBe(500L);

        await _itemRepository.Received(2).UpdateAsync(Arg.Any<MarketDocumentItem>(), Arg.Any<CancellationToken>());

        _fileStorage.UploadedPaths.Count.ShouldBe(1);
        _ftpService.UploadedPaths.Count.ShouldBe(1);
        _ftpService.UploadedPaths[0].ShouldStartWith($"{RemoteDirectory}/");

        var uploadedFtpContent = _ftpService.GetUploadedContent(_ftpService.UploadedPaths[0]);
        uploadedFtpContent.ShouldNotBeNull();
        Encoding.UTF8.GetString(uploadedFtpContent!).ShouldBe(generatedContent);

        var fileStorageOptions = new FileStorageOptions { RootDirectory = RootDirectory };
        await using var roundTripStream = await _fileStorage.OpenReadAsync(fileStorageOptions, created.Asset!.Reference, CancellationToken.None);
        using var reader = new StreamReader(roundTripStream);
        var roundTrippedContent = await reader.ReadToEndAsync(CancellationToken.None);

        roundTrippedContent.ShouldBe(generatedContent);
    }

    [Fact]
    public async Task SkipsConfigurationWhenThereAreNoPendingItems()
    {
        SeedPendingItems([]);

        var configuration = CreateConfiguration(new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" });

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([configuration]);

        var result = await _handler.Handle(new ExportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();

        await _documentRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocument>(), Arg.Any<CancellationToken>());
        _fileStorage.UploadedPaths.ShouldBeEmpty();
        _ftpService.UploadedPaths.ShouldBeEmpty();
    }

    [Fact]
    public async Task ASecondConfigurationDoesNotReClaimItemsTheFirstAlreadyAssigned()
    {
        var pendingItems = new List<MarketDocumentItem>
        {
            new() { Id = 1, Set = "814", SubSet = "01", Raw = "<Transaction/>" },
        };

        SeedPendingItems(pendingItems);
        SeedGeneratedContent("<Document/>");

        var company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" };
        var firstConfiguration = CreateConfiguration(company, name: "edi.export.1");
        var secondConfiguration = CreateConfiguration(company, name: "edi.export.2");

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([firstConfiguration, secondConfiguration]);

        long nextId = 700;
        _documentRepository
            .AddAsync(Arg.Any<MarketDocument>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var document = callInfo.Arg<MarketDocument>();
                document.Id = nextId++;
                return document;
            });

        var result = await _handler.Handle(new ExportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([700]);

        // The second configuration found no remaining pending items, so it never created a document.
        await _documentRepository.Received(1).AddAsync(Arg.Any<MarketDocument>(), Arg.Any<CancellationToken>());
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

    private static Configuration CreateConfiguration(Company company, string name = "edi.export")
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
            Name = name,
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
