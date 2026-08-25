using Core.Common;

namespace Core.Shared;

public class SystemIntegration : AuditableEntityBase<int>
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CompanyId { get; set; }

    public IList<SystemIntegrationField> SystemIntegrationFields { get; set; } = [];
    public Company? Company { get; set; }
}
