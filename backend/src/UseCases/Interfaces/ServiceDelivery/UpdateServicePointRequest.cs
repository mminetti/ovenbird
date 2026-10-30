namespace UseCases.Interfaces.ServiceDelivery;

public sealed class UpdateServicePointRequest(
    string servicePointIdentifier,
    string commodityCode,
    string? customerClassCode,
    string? loadProfileCode,
    string? readCycleCode,
    string? advanceMeterInfoCode,
    string? meterNumber,
    string? premiseAddressLine1,
    string? premiseCity,
    string? premiseState,
    string? premisePostalCode,
    string? countyName)
{
    public string ServicePointIdentifier { get; set; } = servicePointIdentifier;
    public string CommodityCode { get; set; } = commodityCode;
    public string? CustomerClassCode { get; set; } = customerClassCode;
    public string? LoadProfileCode { get; set; } = loadProfileCode;
    public string? ReadCycleCode { get; set; } = readCycleCode;
    public string? AdvanceMeterInfoCode { get; set; } = advanceMeterInfoCode;
    public string? MeterNumber { get; set; } = meterNumber;
    public string? PremiseAddressLine1 { get; set; } = premiseAddressLine1;
    public string? PremiseCity { get; set; } = premiseCity;
    public string? PremiseState { get; set; } = premiseState;
    public string? PremisePostalCode { get; set; } = premisePostalCode;
    public string? CountyName { get; set; } = countyName;
}
