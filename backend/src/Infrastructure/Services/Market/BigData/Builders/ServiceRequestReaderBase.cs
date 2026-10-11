using System.Xml.Linq;
using System.Xml.Serialization;
using Core.Market;
using Core.Subscription;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class ServiceRequestReaderBase
{
    protected const string DateFormat = "yyyyMMdd";

    protected static ServiceRequestXml DeserializeTransaction(MarketDocumentItem item)
    {
        var transactionElement = XElement.Parse(item.Raw);

        if (transactionElement.Element("AccountMaint") is null)
        {
            throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no 'AccountMaint' element.");
        }

        var serializer = new XmlSerializer(typeof(ServiceRequestXml));

        using var reader = transactionElement.CreateReader();

        return (ServiceRequestXml)serializer.Deserialize(reader)!;
    }

    // TODO: no 814 transaction carries an explicit back-reference to Account.Identifier yet.
    // Once a builder exists for the enrollment transaction that originates Account rows (e.g. 814_01)
    // and defines how Account.Identifier is populated, resolve the account here using the matching
    // field from AccountMaintXml instead of throwing.
    protected static Task<Account> GetAccountAsync(AccountMaintXml accountMaint, CancellationToken ct) =>
        throw new NotImplementedException("Account resolution for 814 transactions is not yet implemented.");
}
