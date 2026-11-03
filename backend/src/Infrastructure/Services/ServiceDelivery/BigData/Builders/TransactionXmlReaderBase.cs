using System.Xml;
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
        XElement transactionElement;
        try
        {
            transactionElement = XElement.Parse(item.Raw);
        }
        catch (XmlException ex)
        {
            return Result.Invalid(new ValidationError(nameof(item.Raw), $"MarketDocumentItem '{item.Id}' Raw content is not valid XML: {ex.Message}"));
        }

        if (transactionElement.Element(RequiredElementName) is null)
        {
            return Result.Invalid(new ValidationError(
                RequiredElementName,
                $"'{RequiredElementName}' element is required."));
        }

        var serializer = new XmlSerializer(typeof(TXml));

        using var reader = transactionElement.CreateReader();

        try
        {
            return Result.Success((TXml)serializer.Deserialize(reader)!);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Invalid(new ValidationError(
                RequiredElementName, $"MarketDocumentItem '{item.Id}' could not be deserialized: {ex.Message}"));
        }
    }

    protected static Result<IReadOnlyList<long>> AsResult<T>(Result<T> result) =>
        Result<IReadOnlyList<long>>.Invalid(result.ValidationErrors.ToList());
}
