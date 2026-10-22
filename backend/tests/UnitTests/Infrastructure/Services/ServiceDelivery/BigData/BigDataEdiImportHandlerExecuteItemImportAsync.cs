using System.Text;
using Core.Assets;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.Settings;
using Infrastructure.Services.Files;
using Infrastructure.Services.ServiceDelivery.BigData;
using Infrastructure.Services.ServiceDelivery.BigData.Readers;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.UseCases.Market.MarketDocuments.TestDoubles;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData;

public class BigDataEdiImportHandlerExecuteItemImportAsync
{
    private const string FileStorageImplementation = "TestFileStorage";
    private const string RootDirectory = "import-market-documents";
    private const string FileStorageFolder = "edi/import";

    private readonly TestFileStorage _fileStorage = new();
    private readonly ConnectorFileStorageTransport _transport;
    private readonly BigDataEdiImportHandler _handler;
    private readonly Configuration _configuration;

    public BigDataEdiImportHandlerExecuteItemImportAsync()
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IFileStorage>(FileStorageImplementation, _fileStorage);
        services.AddKeyedSingleton<IMarketDocumentTransactionReader, Usage_867_TransactionReader>(Usage_867_TransactionReader.Set);
        services.AddKeyedSingleton<IMarketDocumentTransactionReader, Invoice_810_TransactionReader>(Invoice_810_TransactionReader.Set);

        var serviceProvider = services.BuildServiceProvider();

        var secretResolver = Substitute.For<IConnectorFieldSecretResolver>();
        secretResolver
            .ResolveAsync(Arg.Any<ConnectorField>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(((ConnectorField)ci[0]).Value!));

        _transport = new ConnectorFileStorageTransport(serviceProvider, secretResolver, TimeProvider.System);

        var ftpTransport = new ConnectorFtpTransport(serviceProvider, secretResolver);
        var parserResolver = new MarketDocumentParserResolver([new BigDataDocumentImportParser(serviceProvider)]);

        _handler = new BigDataEdiImportHandler(
            ftpTransport,
            _transport,
            Substitute.For<IReadRepository<MarketDocument>>(),
            parserResolver);

        _configuration = CreateConfiguration();
    }

    [Fact]
    public async Task ParsesEachTransactionIntoAMarketDocumentItem()
    {
        const string xml = """
            <Document>
              <TransactionList>
                <Transaction>
                  <TransactionSet>867</TransactionSet>
                  <TransactionSubSet>03</TransactionSubSet>
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

        var items = await _handler.ExecuteItemImportAsync(_configuration, document, CancellationToken.None);

        items.Count.ShouldBe(2);

        items[0].Set.ShouldBe("867");
        items[0].ReferenceNumber.ShouldBe("REF-1");
        items[0].ServicePointIdentifier.ShouldBe("ACCT-1");

        items[1].Set.ShouldBe("810");
        items[1].ReferenceNumber.ShouldBe("BILL-1");
        items[1].ServicePointIdentifier.ShouldBe("LDC-1");
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

        await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.ExecuteItemImportAsync(_configuration, document, CancellationToken.None));
    }

    private async Task<MarketDocument> UploadDocumentAsync(string xml)
    {
        using var content = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var fileReference = await _transport.UploadAsync(_configuration, content, "document.xml", CancellationToken.None);

        return new MarketDocument
        {
            Id = 1,
            Identifier = "document.xml",
            Asset = new Asset { FileName = "document.xml", Reference = fileReference },
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
