using System.Xml.Linq;
using System.Xml.Serialization;
using Core.Market;

namespace Infrastructure.Services.Market.BigData.Builders;

public static class UsageTransactionXmlReader
{
    public const string DateFormat = "yyyyMMdd";

    public static UsageTransactionXml DeserializeTransaction(MarketDocumentItem item)
    {
        var transactionElement = XElement.Parse(item.Raw);

        if (transactionElement.Element("Usage") is null)
        {
            throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no 'Usage' element.");
        }

        var serializer = new XmlSerializer(typeof(UsageTransactionXml));

        using var reader = transactionElement.CreateReader();

        return (UsageTransactionXml)serializer.Deserialize(reader)!;
    }
}
