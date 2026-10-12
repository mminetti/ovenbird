using System.Text;
using System.Xml.Linq;
using Core.Market;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace Infrastructure.Services.Market.BigData;

public class BigDataMarketDocumentGenerator : IMarketDocumentGenerator
{
    public string Identifier => "BigData";

    public Task<Stream> WriteTransactionsAsync(IReadOnlyList<MarketDocumentItem> items, CancellationToken ct)
    {
        var transactionList = new XElement("TransactionList", items.Select(item => XElement.Parse(item.Raw)));
        var document = new XElement("Document", transactionList);

        var bytes = Encoding.UTF8.GetBytes(document.ToString());

        return Task.FromResult<Stream>(new MemoryStream(bytes));
    }
}
