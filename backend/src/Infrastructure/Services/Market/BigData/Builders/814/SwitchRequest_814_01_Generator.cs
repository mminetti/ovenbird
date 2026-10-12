namespace Infrastructure.Services.Market.BigData.Builders;

public class SwitchRequest_814_01_Generator(TimeProvider timeProvider) 
    : ServiceStartRequestGeneratorBase(timeProvider)
{
    public const string Key = "814_01";

    protected override string SubSet => "01";

    // Confirmed against a real 814_01 Switch Request sample.
    protected override string ResponseRequestFlag => "Q";
    protected override string ServiceRequested => "CE";
    protected override string SecondServiceRequested => "HU";
    protected override string? ThirdServiceRequested => "SW";
    protected override string BillingOption => "ESP";
    protected override string BillCalculator => "DUAL";

    protected override ServiceDateXml BuildServiceDate(string requestedDate) =>
        new() { SpecialMeterReadDate = requestedDate };
}
