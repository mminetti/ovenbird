using Core.Auditing;

namespace UseCases.Auditing;

public record AuditTrailDto(
    long Id,
    string EntityType,
    string EntityId,
    AuditAction Action,
    string? UserId,
    DateTimeOffset TimestampUtc,
    string? OldValues,
    string? NewValues);
