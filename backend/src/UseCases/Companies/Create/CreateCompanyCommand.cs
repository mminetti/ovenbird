namespace UseCases.Companies.Create;

public record CreateCompanyCommand(
    string Name,
    int MarketId,
    string TimeZoneId);
