namespace UseCases.Companies;

public record CompanyDto(
    int Id,
    string Name,
    int MarketId,
    string MarketName,
    string TimeZoneId,
    DateTimeOffset LastModifiedAtUtc,
    string? LastModifiedBy);
