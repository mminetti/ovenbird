namespace UseCases.Settings.Companies.Create;

public record CreateCompanyCommand(
    string Name,
    int MarketId,
    string TimeZoneId);
