namespace Web.Endpoints.Auditing;

public record AuditTrailRecord(
    long Id,
    string EntityType,
    string EntityId,
    string Action,
    string? UserId,
    DateTimeOffset TimestampUtc,
    string? OldValues,
    string? NewValues);
