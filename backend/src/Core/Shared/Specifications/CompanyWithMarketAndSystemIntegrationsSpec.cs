namespace Core.Shared.Specifications;

public class CompanyWithMarketAndSystemIntegrationsSpec : Specification<Company>
{
    public CompanyWithMarketAndSystemIntegrationsSpec() =>
        Query
            .Include(company => company.Market)
            .Include(company => company.SystemIntegrations)
            .ThenInclude(si => si.SystemIntegrationFields);
}
