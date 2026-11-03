namespace UseCases.Interfaces.ServiceDelivery;

public sealed class UpdateServicePointRequest(
    string servicePointIdentifier,
    string commodityCode,
    string? customerClassCode,
    string? loadProfileCode,
    string? readCycleCode,
    string? amsMeterCode,
    string? meterNumber,
    string? premiseAddressLine1,
    string? premiseCity,
    string? premiseState,
    string? premiseZipCode,
    string? countyName)
{
    public string ServicePointIdentifier { get; set; } = servicePointIdentifier;
    public string CommodityCode { get; set; } = commodityCode;
    public string? CustomerClassCode { get; set; } = customerClassCode;
    public string? LoadProfileCode { get; set; } = loadProfileCode;
    public string? ReadCycleCode { get; set; } = readCycleCode;
    public string? AmsMeterCode { get; set; } = amsMeterCode;
    public string? MeterNumber { get; set; } = meterNumber;
    public string? PremiseAddressLine1 { get; set; } = premiseAddressLine1;
    public string? PremiseCity { get; set; } = premiseCity;
    public string? PremiseState { get; set; } = premiseState;
    public string? PremiseZipCode { get; set; } = premiseZipCode;
    public string? CountyName { get; set; } = countyName;
}
