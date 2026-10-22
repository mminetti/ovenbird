using System.Xml.Linq;
using System.Xml.Serialization;
using Core.MarketExchange;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public abstract class TransactionXmlReaderBase<TXml>
{
    protected const string DateFormat = "yyyyMMdd";

    protected abstract string RequiredElementName { get; }

    protected Result<TXml> DeserializeTransaction(MarketDocumentItem item)
    {
        var transactionElement = XElement.Parse(item.Raw);

        if (transactionElement.Element(RequiredElementName) is null)
        {
            return Result.Invalid(new ValidationError(
                RequiredElementName,
                $"MarketDocumentItem '{item.Id}' has no '{RequiredElementName}' element."));
        }

        var serializer = new XmlSerializer(typeof(TXml));

        using var reader = transactionElement.CreateReader();

        return Result.Success((TXml)serializer.Deserialize(reader)!);
    }

    protected static Result AsResult<T>(Result<T> result) =>
        Result.Invalid(result.ValidationErrors.ToList());
}
