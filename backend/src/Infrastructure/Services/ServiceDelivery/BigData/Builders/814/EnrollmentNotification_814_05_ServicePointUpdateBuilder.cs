using Core.MarketExchange;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class EnrollmentNotification_814_05_ServicePointUpdateBuilder
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IServicePointUpdateBuilder
{
    private const string PremiseEntityType = "8R";
    protected override string RequiredElementName => "AccountMaint";

    public Task<Result<UpdateServicePointRequest>> BuildAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionResult = DeserializeTransaction(item);
        if (!transactionResult.IsSuccess)
        {
            return Task.FromResult(Result<UpdateServicePointRequest>.Invalid(transactionResult.ValidationErrors.ToList()));
        }

        var transaction = transactionResult.Value;
        var accountMaint = transaction.AccountMaint;

        var service = accountMaint.Services.FirstOrDefault();
        if (service is null)
        {
            return Task.FromResult(Result<UpdateServicePointRequest>.Invalid(
                new ValidationError(nameof(accountMaint.Services), "Service is required.")));
        }

        var premise = accountMaint.NameAddresses.FirstOrDefault(x => x.EntityType == PremiseEntityType);
        var serviceMeter = service.ServiceMeters.FirstOrDefault();

        var request = new UpdateServicePointRequest(
            service.UtilityAccountNumber,
            transaction.Commodity,
            serviceMeter?.LDCRateClass,
            serviceMeter?.LoadProfileCode,
            serviceMeter?.OwnerMeterCycle,
            serviceMeter?.AdvanceMeterInfo,
            serviceMeter?.MeterNumber,
            premise?.EntityAddress1,
            premise?.EntityCity,
            premise?.EntityState,
            premise?.EntityPostalCode,
            premise?.EntityCounty);

        return Task.FromResult(Result<UpdateServicePointRequest>.Success(request));
    }
}
