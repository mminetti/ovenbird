namespace Core.ServiceDelivery.Events;

public sealed class ServiceStartScheduledEvent(
    long serviceStartId,
    DateOnly serviceDate,
    string servicePointIdentifier,
    int commodityId,
    int customerClassId,
    int loadProfileId,
    int readCycleId,
    bool isAmsMeter,
    string meterIdentifier,
    string addressLine1,
    string city,
    string state,
    string zip,
    string? zip4,
    int countyId) : DomainEventBase
{
    public long ServiceStartId { get; set; } = serviceStartId;
    public DateOnly ServiceDate { get; set; } = serviceDate;
    public string ServicePointIdentifier { get; set; } = servicePointIdentifier;
    public int CommodityId { get; set; } = commodityId;
    public int CustomerClassId { get; set; } = customerClassId;
    public int LoadProfileId { get; set; } = loadProfileId;
    public int ReadCycleId { get; set; } = readCycleId;
    public bool IsAmsMeter { get; set; } = isAmsMeter;
    public string MeterIdentifier { get; set; } = meterIdentifier;
    public string AddressLine1 { get; set; } = addressLine1;
    public string City { get; set; } = city;
    public string State { get; set; } = state;
    public string Zip { get; set; } = zip;
    public string? Zip4 { get; set; } = zip4;
    public int CountyId { get; set; } = countyId;
}
