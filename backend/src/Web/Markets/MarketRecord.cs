namespace Web.Markets;

public record MarketRecord(
    int Id,
    string Name,
    string Identifier,
    DateTimeOffset LastModifiedAtUtc,
    string? LastModifiedBy);
