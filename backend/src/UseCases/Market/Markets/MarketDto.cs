namespace UseCases.Market.Markets;

public record MarketDto(
    int Id,
    string Name,
    string Identifier,
    DateTimeOffset LastModifiedAtUtc,
    string? LastModifiedBy);
