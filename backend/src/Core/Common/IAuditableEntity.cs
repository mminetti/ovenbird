namespace Core.Common;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTimeOffset LastModifiedAtUtc { get; set; }
    string? LastModifiedBy { get; set; }
}
