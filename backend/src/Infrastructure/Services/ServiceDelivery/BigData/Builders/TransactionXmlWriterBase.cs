using System.Xml;
using System.Xml.Serialization;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public abstract class TransactionXmlWriterBase<TXml>
{
    protected const string DateFormat = "yyyyMMdd";

    protected static string SerializeTransaction(TXml transaction)
    {
        var serializer = new XmlSerializer(typeof(TXml));
        var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false };

        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        serializer.Serialize(xmlWriter, transaction, namespaces);

        return stringWriter.ToString();
    }
}
