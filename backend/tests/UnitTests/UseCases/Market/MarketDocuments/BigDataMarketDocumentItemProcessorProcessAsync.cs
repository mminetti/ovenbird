using System.Text;
using System.Xml.Linq;
using Core.Common.Constants;
using Core.Market;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Market.MarketDocuments.Import.BigData;
using UseCases.Market.MarketDocuments.Import.BigData.Readers;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class BigDataMarketDocumentItemProcessorProcessAsync
{
    [Fact]
    public async Task DispatchesEachTransactionToTheReaderMatchingItsTransactionSet()
    {
        var usageReader = Substitute.For<IMarketDocumentTransactionReader>();
        usageReader.Read(Arg.Any<XElement>()).Returns(_ => new MarketDocumentItem());

        var invoiceReader = Substitute.For<IMarketDocumentTransactionReader>();
        invoiceReader.Read(Arg.Any<XElement>()).Returns(_ => new MarketDocumentItem());

        var services = new ServiceCollection();
        services.AddKeyedSingleton("867", usageReader);
        services.AddKeyedSingleton("810", invoiceReader);

        var processor = new BigDataMarketDocumentItemProcessor(services.BuildServiceProvider());

        const string xml = """
            <Document>
              <TransactionList>
                <Transaction>
                  <TransactionSet>867</TransactionSet>
                  <TransactionSubSet>04</TransactionSubSet>
                  <Usage><PurposeCode>SU</PurposeCode></Usage>
                </Transaction>
                <Transaction>
                  <TransactionSet>810</TransactionSet>
                  <TransactionSubSet>02</TransactionSubSet>
                  <Invoice><BillPurpose>00</BillPurpose></Invoice>
                </Transaction>
              </TransactionList>
            </Document>
            """;

        using var content = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var items = await processor.ProcessAsync(content, CancellationToken.None);

        items.Count.ShouldBe(2);

        items[0].Set.ShouldBe("867");
        items[0].SubSet.ShouldBe("04");
        items[0].MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.New);
        items[0].Raw.ShouldContain("<TransactionSet>867</TransactionSet>");

        items[1].Set.ShouldBe("810");
        items[1].SubSet.ShouldBe("02");
        items[1].MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.New);
        items[1].Raw.ShouldContain("<TransactionSet>810</TransactionSet>");

        usageReader.Received(1).Read(Arg.Is<XElement>(e => e.Element("TransactionSet")!.Value == "867"));
        invoiceReader.Received(1).Read(Arg.Is<XElement>(e => e.Element("TransactionSet")!.Value == "810"));
    }

    [Fact]
    public async Task PropagatesExceptionWhenTransactionSetCannotBeResolved()
    {
        var services = new ServiceCollection();
        var processor = new BigDataMarketDocumentItemProcessor(services.BuildServiceProvider());

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

        using var content = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        await Should.ThrowAsync<InvalidOperationException>(
            () => processor.ProcessAsync(content, CancellationToken.None));
    }
}
