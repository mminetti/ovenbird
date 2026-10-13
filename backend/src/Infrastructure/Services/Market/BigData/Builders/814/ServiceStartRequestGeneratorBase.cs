using Core.Market;
using Core.Service;
using Core.Subscription;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class ServiceStartRequestGeneratorBase(TimeProvider timeProvider)
    : TransactionXmlWriterBase<ServiceRequestXml>, IMarketDocumentEntityGenerator
{
    private const string Set = "814";
    private const string TimeFormat = "HHmmss";
    private const string CommodityCode = "E";
    private const string ProductId = "EL";
    private const string ResponseRequestFlag = "Q";
    private const string ServiceRequested = "CE";
    private const string SecondServiceRequested = "HU";
    private const string BillingOption = "ESP";
    private const string BillCalculator = "DUAL";
    private const string State = "TX";

    protected abstract string SubSet { get; }

    protected virtual string? ThirdServiceRequested => null;
    protected virtual string? PriorityCode => null;

    protected abstract ServiceDateXml BuildServiceDate(string requestedDate);

    public Task<MarketDocumentItem> CreateAsync(ServiceStartItem serviceStartItem, CancellationToken ct)
    {
        var serviceStart = serviceStartItem.ServiceStart;
        var account = serviceStart.Account;

        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        var servicePoint = account.ServicePoint;
        var customer = account.Customer;

        var now = timeProvider.GetUtcNow();
        var requestedDate = (serviceStartItem.ServiceDate ?? serviceStart.ServiceDate).ToString(DateFormat);

        var transaction = new ServiceRequestXml
        {
            // TODO: SupplierDUNS (this company's own DUNS) and UtilityDUNS (the target
            // utility's DUNS) are not stored anywhere in the domain model today - neither
            // Core.Settings.Company nor Core.Service.Utility has a DUNS field. Must be
            // filled in (e.g. by adding a DunsNumber field to Utility/Company) before this
            // can be sent to a trading partner.
            SupplierDUNS = string.Empty,
            UtilityDUNS = string.Empty,
            TransactionSet = Set,
            TransactionSubSet = SubSet,
            State = State,
            Commodity = CommodityCode,
            AccountMaint = new AccountMaintXml
            {
                ResponseRequestFlag = ResponseRequestFlag,
                TransactionNumber = serviceStartItem.Identifier,
                UtilityName = servicePoint.Utility.Name,
                // TODO: ESPName is this company's own trading-partner-registered display
                // name; not available to this generator (only the ServiceStartItem is
                // passed in, not the Company/Configuration it will eventually ship through).
                ESPName = null,
                DateStampCreated = now.ToString(DateFormat),
                TimeStampCreated = now.ToString(TimeFormat),
                NameAddresses = BuildNameAddresses(customer, servicePoint.Premise),
                Services =
                [
                    new ServiceXml
                    {
                        ProductID = ProductId,
                        ServiceRequested = ServiceRequested,
                        SecondServiceRequested = SecondServiceRequested,
                        ThirdServiceRequested = ThirdServiceRequested,
                        LineTrackNumber = "1",
                        UtilityAccountNumber = servicePoint?.Identifier ?? string.Empty,
                        BillingOption = BillingOption,
                        BillCalculator = BillCalculator,
                        PriorityCode = PriorityCode,
                        SpecialNeedsInfo = servicePoint?.IsCriticalCare == true ? "Y" : "N",
                        ServiceDates = [BuildServiceDate(requestedDate)],
                    }
                ],
            },
        };

        var raw = SerializeTransaction(transaction);

        var item = new MarketDocumentItem
        {
            Raw = raw,
            ReferenceNumber = transaction.AccountMaint.TransactionNumber,
            ServicePointIdentifier = transaction.AccountMaint.Services[0].UtilityAccountNumber,
        };

        return Task.FromResult(item);
    }

    // The trading partner's samples repeat the same customer/address under three entity
    // types (8R, N1, PO) - we only model one party per account today, so all three entries
    // are built from the same Customer/Premise data.
    private static List<NameAddressXml> BuildNameAddresses(Customer customer, Premise premise)
    {
        if (customer is null || premise is null)
        {
            return [];
        }

        var contactName = $"{customer.FirstName} {customer.LastName}".Trim();

        NameAddressXml Build(string entityType) => new()
        {
            EntityType = entityType,
            EntityName = contactName,
            EntityAddress1 = premise.Address,
            EntityCity = premise.City,
            EntityState = premise.State,
            EntityPostalCode = premise.Zip,
            EntityContactName = contactName,
            // TODO: EntityTelephone/EntityEmail/EntityMobilePhone are not modeled on
            // Core.Subscription.Customer today.
        };

        return [Build("8R"), Build("N1"), Build("PO")];
    }
}
