namespace Core.Common;

public class CaseItem : AuditableEntityBase<long>
{
    public long CaseId { get; set; }
    public int SeverityId { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public Case Case { get; set; } = default!;
    public ErrorSeverity Severity { get; set; } = default!;
}
