using Core.Common;

namespace Core.ServiceDelivery;

public class Premise : AuditableEntityBase<long>
{
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string? Zip4 { get; set; }
    public int? CountyId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public County? County { get; set; }
}
