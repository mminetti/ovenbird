namespace Web.Companies;

public record CompanyRecord(
    int Id,
    string Name,
    int MarketId,
    string MarketName,
    string TimeZoneId,
    DateTimeOffset LastModifiedAtUtc,
    string? LastModifiedBy);
