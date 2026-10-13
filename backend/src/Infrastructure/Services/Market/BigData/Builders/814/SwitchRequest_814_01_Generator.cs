namespace Infrastructure.Services.Market.BigData.Builders;

public class SwitchRequest_814_01_Generator(TimeProvider timeProvider) 
    : ServiceStartRequestGeneratorBase(timeProvider)
{
    public const string Key = "814_01";

    protected override string SubSet => "01";
    protected override string? ThirdServiceRequested => "SW";

    protected override ServiceDateXml BuildServiceDate(string requestedDate) =>
        new() { SpecialMeterReadDate = requestedDate };
}
