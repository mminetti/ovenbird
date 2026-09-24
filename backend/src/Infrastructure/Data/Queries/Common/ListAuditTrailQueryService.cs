using Core.Common;
using UseCases.Auditing;
using UseCases.Auditing.List;
using UseCases.Common;

namespace Infrastructure.Data.Queries.Common;

public class ListAuditTrailQueryService(ReadDbContext db) : IListAuditTrailQueryService
{
    public async Task<ItemPagedResult<AuditTrailDto>> ListAsync(string entityType, string entityId, int page, int perPage, CancellationToken ct)
    {
        var query = db.AuditTrail
            .Where(a =>
                (a.EntityType == entityType && a.EntityId == entityId) ||
                a.References.Any(r => r.ReferencedEntityType == entityType && r.ReferencedEntityId == entityId));

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.TimestampUtc)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(a => new AuditTrailDto(
                a.Id,
                a.EntityType,
                a.EntityId,
                a.Action,
                a.UserId,
                a.TimestampUtc,
                a.OldValues,
                a.NewValues,
                a.AffectedColumns))
            .AsNoTracking()
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

        return new ItemPagedResult<AuditTrailDto>(items, page, perPage, totalCount, totalPages);
    }
}
