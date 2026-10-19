namespace Infrastructure.Services.Market.BigData.Builders;

public class SwitchRequest_814_01_ExportGenerator(TimeProvider timeProvider)
    : ServiceStartRequestExportGeneratorBase(timeProvider)
{
    public const string Key = "BigData_814_01";

    protected override string SubSet => "01";
    protected override string? ThirdServiceRequested => "SW";

    protected override ServiceDateXml BuildServiceDate(string requestedDate) =>
        new() { SpecialMeterReadDate = requestedDate };
}
