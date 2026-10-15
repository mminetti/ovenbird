using System.Xml.Linq;
using System.Xml.Serialization;
using Core.MarketExchange;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class TransactionXmlReaderBase<TXml>
{
    protected const string DateFormat = "yyyyMMdd";

    protected abstract string RequiredElementName { get; }

    protected TXml DeserializeTransaction(MarketDocumentItem item)
    {
        var transactionElement = XElement.Parse(item.Raw);

        if (transactionElement.Element(RequiredElementName) is null)
        {
            throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no '{RequiredElementName}' element.");
        }

        var serializer = new XmlSerializer(typeof(TXml));

        using var reader = transactionElement.CreateReader();

        return (TXml)serializer.Deserialize(reader)!;
    }
}
