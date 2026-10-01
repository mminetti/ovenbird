namespace UseCases.Settings.Companies.Update;

public record UpdateCompanyCommand(
    int CompanyId,
    string Name,
    int MarketId,
    string TimeZoneId);
