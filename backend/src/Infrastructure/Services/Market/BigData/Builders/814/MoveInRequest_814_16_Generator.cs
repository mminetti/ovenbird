namespace Infrastructure.Services.Market.BigData.Builders;

public class MoveInRequest_814_16_Generator(TimeProvider timeProvider) : ServiceStartRequestGeneratorBase(timeProvider)
{
    public const string Key = "814_16";

    protected override string SubSet => "16";

    // Confirmed against a real 814_16 Move In Request sample.
    protected override string ResponseRequestFlag => "Q";
    protected override string ServiceRequested => "CE";
    protected override string SecondServiceRequested => "HU";
    protected override string BillingOption => "ESP";
    protected override string BillCalculator => "DUAL";
    protected override string? PriorityCode => "01";

    protected override ServiceDateXml BuildServiceDate(string requestedDate) =>
        new()
        {
            // TODO: CustomerAgreementDate/Time represent when the customer agreed to the
            // move-in, which this domain doesn't track separately from the requested move-in
            // date itself - CustomerAgreementTime and CustomerTimeZone are left unset rather
            // than fabricated.
            CustomerAgreementDate = requestedDate,
            MoveInDate = requestedDate,
        };
}
