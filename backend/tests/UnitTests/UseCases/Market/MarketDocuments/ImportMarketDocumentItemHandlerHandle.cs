using System.Text;
using Core.Common.Constants;
using Core.Market;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.UseCases.Market.MarketDocuments.TestDoubles;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.BigData;
using UseCases.Market.MarketDocuments.Import.BigData.Readers;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class ImportMarketDocumentItemHandlerHandle
{
    private const string FileStorageImplementation = "TestFileStorage";
    private const string RootDirectory = "import-market-documents";
    private const string FileStorageFolder = "edi/import";

    private readonly IRepository<MarketDocumentItem> _itemRepository = Substitute.For<IRepository<MarketDocumentItem>>();
    private readonly IReadRepository<MarketDocument> _documentReadRepository = Substitute.For<IReadRepository<MarketDocument>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly TestFileStorage _fileStorage = new();
    private readonly BigDataMarketDocumentProcessor _strategy;
    private readonly Configuration _configuration;
    private readonly ImportMarketDocumentItemHandler _handler;

    public ImportMarketDocumentItemHandlerHandle()
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IFileStorage>(FileStorageImplementation, _fileStorage);
        services.AddKeyedSingleton<IMarketDocumentTransactionReader, UsageTransactionReader>(UsageTransactionReader.TransactionSet);
        services.AddKeyedSingleton<IMarketDocumentTransactionReader, InvoiceTransactionReader>(InvoiceTransactionReader.TransactionSet);

        var serviceProvider = services.BuildServiceProvider();

        var secretResolver = Substitute.For<IConnectorFieldSecretResolver>();
        secretResolver
            .ResolveAsync(Arg.Any<ConnectorField>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(((ConnectorField)ci[0]).Value!));

        _strategy = new BigDataMarketDocumentProcessor(serviceProvider, secretResolver, TimeProvider.System);

        var strategyResolver = new MarketDocumentProcessorResolver([_strategy]);

        var processorResolver = new MarketDocumentItemProcessorResolver(
            [new BigDataMarketDocumentItemProcessor(serviceProvider)]);

        _configuration = CreateConfiguration();

        _configurationReadRepository
            .GetByIdAsync(_configuration.Id, Arg.Any<CancellationToken>())
            .Returns(_configuration);

        _handler = new ImportMarketDocumentItemHandler(
            _itemRepository,
            _documentReadRepository,
            _configurationReadRepository,
            strategyResolver,
            processorResolver);
    }

    [Fact]
    public async Task ParsesEachTransactionIntoAMarketDocumentItem()
    {
        const string xml = """
            <Document>
              <TransactionList>
                <Transaction>
                  <TransactionSet>867</TransactionSet>
                  <TransactionSubSet>04</TransactionSubSet>
                  <Usage>
                    <PurposeCode>SU</PurposeCode>
                    <TransactionReferenceNumber>REF-1</TransactionReferenceNumber>
                    <UtilityAccountNumber>ACCT-1</UtilityAccountNumber>
                  </Usage>
                </Transaction>
                <Transaction>
                  <TransactionSet>810</TransactionSet>
                  <TransactionSubSet>02</TransactionSubSet>
                  <Invoice>
                    <BillPurpose>00</BillPurpose>
                    <BillActionCode>PR</BillActionCode>
                    <BillNumber>BILL-1</BillNumber>
                    <LDCAccountNumber>LDC-1</LDCAccountNumber>
                  </Invoice>
                </Transaction>
              </TransactionList>
            </Document>
            """;

        var document = await UploadDocumentAsync(xml);

        _documentReadRepository
            .GetByIdAsync(document.Id, Arg.Any<CancellationToken>())
            .Returns(document);

        long nextId = 500;
        var created = new List<MarketDocumentItem>();
        _itemRepository
            .AddAsync(Arg.Do<MarketDocumentItem>(i => created.Add(i)), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var item = callInfo.Arg<MarketDocumentItem>();
                item.Id = nextId++;
                return item;
            });

        var result = await _handler.Handle(
            new ImportMarketDocumentItemCommand(_configuration.Id, document.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([500, 501]);

        created.Count.ShouldBe(2);

        created[0].MarketDocumentId.ShouldBe(document.Id);
        created[0].Set.ShouldBe("867");
        created[0].TrackingNumber.ShouldBe("REF-1");
        created[0].ServicePointIdentifier.ShouldBe("ACCT-1");

        created[1].MarketDocumentId.ShouldBe(document.Id);
        created[1].Set.ShouldBe("810");
        created[1].TrackingNumber.ShouldBe("BILL-1");
        created[1].ServicePointIdentifier.ShouldBe("LDC-1");
    }

    [Fact]
    public async Task ThrowsWhenTransactionSetCannotBeResolved()
    {
        const string xml = """
            <Document>
              <TransactionList>
                <Transaction>
                  <TransactionSet>999</TransactionSet>
                  <TransactionSubSet>01</TransactionSubSet>
                </Transaction>
              </TransactionList>
            </Document>
            """;

        var document = await UploadDocumentAsync(xml);

        _documentReadRepository
            .GetByIdAsync(document.Id, Arg.Any<CancellationToken>())
            .Returns(document);

        await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
            new ImportMarketDocumentItemCommand(_configuration.Id, document.Id), CancellationToken.None));
    }

    private async Task<MarketDocument> UploadDocumentAsync(string xml)
    {
        using var content = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var fileReference = await _strategy.UploadDocumentAsync(_configuration, content, "document.xml", CancellationToken.None);

        return new MarketDocument
        {
            Id = 1,
            Name = "document.xml",
            File = fileReference,
            CompanyId = _configuration.CompanyId!.Value,
        };
    }

    private static Configuration CreateConfiguration()
    {
        var fileStorageConnector = new Connector
        {
            Id = 1,
            Name = "FileStorage",
            ConnectorImplementation = new ConnectorImplementation
            {
                Id = 1,
                Name = "Test File Storage",
                Identifier = FileStorageImplementation,
                ConnectorTypeId = Constants.ConnectorTypes.FileStorage,
                ConnectorType = new ConnectorType { Id = Constants.ConnectorTypes.FileStorage, Name = "File Storage" },
            },
            ConnectorFields =
            [
                new ConnectorField { ConnectorId = 1, Name = "connection.string", Value = "UseDevelopmentStorage=true" },
            ],
        };

        var company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" };

        return new Configuration
        {
            Id = 1,
            Name = "edi.import",
            Company = company,
            CompanyId = company.Id,
            Connectors = [fileStorageConnector],
            ConfigurationFields =
            [
                new ConfigurationField { ConfigurationId = 1, Name = "handler", Value = "BigData" },
                new ConfigurationField { ConfigurationId = 1, Name = "file.storage.root.directory", Value = RootDirectory },
                new ConfigurationField { ConfigurationId = 1, Name = "file.storage.folder", Value = FileStorageFolder },
            ],
        };
    }
}
