using Core.Market;
using Core.Service;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class ServiceStartRequestGeneratorBase
    : TransactionXmlWriterBase<ServiceRequestXml>, IMarketDocumentEntityGenerator
{
    protected abstract string SubSet { get; }

    // TODO: ActionCode for an Initiate request is trading-partner-specific and not
    // discoverable from the codebase today - same unresolved gap as
    // ServiceStartRejectBuilderBase.GetServiceStartAsync on the inbound side.
    protected abstract string ActionCode { get; }

    public Task<MarketDocumentItem> CreateAsync(ServiceStartItem serviceStartItem, CancellationToken ct)
    {
        var serviceStart = serviceStartItem.ServiceStart;
        var account = serviceStart.Account;

        // TODO: SupplierDUNS/UtilityDUNS, State, Commodity, and NameAddress population are
        // trading-partner-specific and not discoverable from the codebase today.
        var transaction = new ServiceRequestXml
        {
            TransactionSet = "814",
            TransactionSubSet = SubSet,
            AccountMaint = new AccountMaintXml
            {
                TransactionNumber = serviceStartItem.Identifier,
                Services =
                [
                    new ServiceXml
                    {
                        ActionCode = ActionCode,
                        UtilityAccountNumber = account?.Identifier ?? string.Empty,
                        ServiceDates =
                        [
                            new ServiceDateXml
                            {
                                // TODO: confirm which ServiceDateXml field represents the requested
                                // service start date for an Initiate request; CustomerAgreementDate
                                // used as a placeholder pending the trading partner's 814 spec.
                                CustomerAgreementDate =
                                    (serviceStartItem.ServiceDate ?? serviceStart.ServiceDate).ToString(DateFormat),
                            }
                        ],
                    }
                ],
            },
        };

        var raw = SerializeTransaction(transaction);

        var item = new MarketDocumentItem
        {
            Raw = raw,
            ReferenceNumber = transaction.AccountMaint.TransactionNumber,
            ServicePointIdentifier = transaction.AccountMaint.Services[0].UtilityAccountNumber ?? string.Empty,
        };

        return Task.FromResult(item);
    }
}
