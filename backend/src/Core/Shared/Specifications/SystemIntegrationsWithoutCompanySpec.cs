namespace Core.Shared.Specifications;

public class SystemIntegrationsWithoutCompanySpec : Specification<SystemIntegration>
{
    public SystemIntegrationsWithoutCompanySpec() =>
        Query
            .Where(si => si.CompanyId == null)
            .Include(si => si.SystemIntegrationFields);
}
