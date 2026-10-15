using System.Xml.Linq;
using Core.MarketExchange;
using Infrastructure.Services.Market.BigData;
using Infrastructure.Services.Market.BigData.Readers;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Infrastructure.Services.Market.BigData;

public class BigDataMarketDocumentGeneratorWriteTransactionsAsync
{
    private readonly BigDataMarketDocumentGenerator _generator = new();

    [Fact]
    public void IdentifierIsBigData()
    {
        _generator.Identifier.ShouldBe("BigData");
    }

    [Fact]
    public async Task WrapsEachItemsRawTransactionInATransactionListAndRoundTripsThroughTheParser()
    {
        var items = new List<MarketDocumentItem>
        {
            new() { Raw = "<Transaction><TransactionSet>814</TransactionSet><TransactionSubSet>01</TransactionSubSet></Transaction>" },
            new() { Raw = "<Transaction><TransactionSet>814</TransactionSet><TransactionSubSet>16</TransactionSubSet></Transaction>" },
        };

        using var content = await _generator.WriteTransactionsAsync(items, CancellationToken.None);

        content.Position = 0;
        var document = await XDocument.LoadAsync(content, LoadOptions.None, CancellationToken.None);

        var transactions = document.Root?.Element("TransactionList")?.Elements("Transaction").ToList();
        transactions.ShouldNotBeNull();
        transactions!.Count.ShouldBe(2);

        // Round-trip through the inbound parser to confirm the wrapper shape it expects is reproduced exactly.
        content.Position = 0;

        var services = new ServiceCollection();
        services.AddKeyedSingleton<IMarketDocumentTransactionReader, ServiceRequest_814_TransactionReader>(
            ServiceRequest_814_TransactionReader.Set);

        var parser = new BigDataMarketDocumentParser(services.BuildServiceProvider());
        var parsedItems = await parser.ReadTransactionsAsync(content, CancellationToken.None);

        parsedItems.Count.ShouldBe(2);
        parsedItems[0].SubSet.ShouldBe("01");
        parsedItems[1].SubSet.ShouldBe("16");
    }

    [Fact]
    public async Task ReturnsAnEmptyTransactionListWhenThereAreNoItems()
    {
        using var content = await _generator.WriteTransactionsAsync([], CancellationToken.None);

        content.Position = 0;
        var document = await XDocument.LoadAsync(content, LoadOptions.None, CancellationToken.None);

        document.Root?.Element("TransactionList")?.Elements("Transaction").ShouldBeEmpty();
    }
}
