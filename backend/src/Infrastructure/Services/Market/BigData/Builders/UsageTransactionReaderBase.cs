using Core.Market;
using Core.Subscription;
using Core.Subscription.Specifications;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class UsageTransactionReaderBase(IReadRepository<Account> accountReadRepository)
{
    protected const string DateFormat = UsageTransactionXmlReader.DateFormat;

    protected static UsageTransactionXml DeserializeTransaction(MarketDocumentItem item) =>
        UsageTransactionXmlReader.DeserializeTransaction(item);

    protected async Task<Account> GetAccountAsync(string? originalReferenceNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(originalReferenceNumber))
        {
            throw new InvalidOperationException("Usage transaction is missing original reference number.");
        }

        // TODO: this should get the service start by identifier and return the account id

        var account = await accountReadRepository.FirstOrDefaultAsync(new AccountByIdentifierSpec(originalReferenceNumber), ct)
            ?? throw new InvalidOperationException($"Account '{originalReferenceNumber}' was not found.");

        return account;
    }
}
