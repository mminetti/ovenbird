namespace Core.Common;

public abstract class ErrorLogItem : AuditableEntityBase<long>
{
    public int SeverityId { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ErrorSeverity Severity { get; set; } = default!;
}
