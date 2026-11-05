using Core.Security;

namespace Core.Common;

public class Case : AuditableEntityBase<long>
{
    public int EntityTypeId { get; set; }
    public long EntityId { get; set; }
    public int StatusId { get; set; }
    public int? AssignedToUserId { get; set; }

    public CaseEntityType EntityType { get; set; } = default!;
    public CaseStatus Status { get; set; } = default!;
    public User? AssignedToUser { get; set; }
    public ICollection<CaseItem> Items { get; set; } = [];
}
