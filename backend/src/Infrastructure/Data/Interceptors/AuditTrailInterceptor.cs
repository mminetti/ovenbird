using System.Text.Json;
using Core.Common;
using Core.Security;
using Core.Shared;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UseCases.Common;

namespace Infrastructure.Data.Interceptors;

public class AuditTrailInterceptor(TimeProvider dateTime, IUser user) : SaveChangesInterceptor
{
    // Shared-type (join) entities have no CLR class to implement IAudited on, so their
    // opt-in is this explicit name list instead of the marker interface.
    private static readonly HashSet<string> _auditedSharedTypeEntities =
    [
        "UserRole",
        "RolePermission"
    ];

    // IAuditableEntity's stamps (CreatedBy/CreatedAtUtc/LastModifiedBy/LastModifiedAtUtc) are
    // redundant with the AuditTrail row's own UserId/TimestampUtc, so they're excluded from
    // values and affected-columns rather than cluttering every row.
    private static readonly HashSet<string> _excludedProperties =
    [
        nameof(IAuditableEntity.CreatedBy),
        nameof(IAuditableEntity.CreatedAtUtc),
        nameof(IAuditableEntity.LastModifiedBy),
        nameof(IAuditableEntity.LastModifiedAtUtc)
    ];

    // Declares, per audited entity-type name, which FK properties point to entities worth
    // recording as references - either the other side of a join row, or the parent of a
    // child-entity rollup. Only consulted for entries that already pass IsAudited.
    private static readonly Dictionary<string, (string FkProperty, string ReferencedType)[]> ReferenceMap = new()
    {
        ["UserRole"] = [("UserId", nameof(User)), ("RoleId", nameof(Role))],
        ["RolePermission"] = [("RoleId", nameof(Role)), ("PermissionId", nameof(Permission))],
        [nameof(ConfigurationField)] = [("ConfigurationId", nameof(Configuration))],
        [nameof(ConnectorField)] = [("ConnectorId", nameof(Connector))]
    };

    private readonly TimeProvider _dateTime = dateTime;
    private readonly IUser _user = user;
    private readonly List<(AuditTrail Row, EntityEntry Entry)> _pendingCreates = [];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            CaptureAuditTrail(context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context && _pendingCreates.Count > 0)
        {
            foreach (var (row, entry) in _pendingCreates)
            {
                row.EntityId = GetEntityId(entry, useCurrentValues: true);
            }

            _pendingCreates.Clear();

            await context.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void CaptureAuditTrail(DbContext context)
    {
        _pendingCreates.Clear();

        var utcNow = _dateTime.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            if (entry.Entity is AuditTrail or AuditTrailReference)
            {
                continue;
            }

            if (entry.State is EntityState.Unchanged or EntityState.Detached)
            {
                continue;
            }

            if (!IsAudited(entry))
            {
                continue;
            }

            var row = BuildAuditRow(entry, utcNow);

            if (row is null)
            {
                continue;
            }

            context.Set<AuditTrail>().Add(row);

            if (entry.State == EntityState.Added)
            {
                _pendingCreates.Add((row, entry));
            }
        }
    }

    private AuditTrail? BuildAuditRow(EntityEntry entry, DateTimeOffset utcNow)
    {
        var entityType = GetEntityTypeName(entry);

        switch (entry.State)
        {
            case EntityState.Added:
                return new AuditTrail
                {
                    EntityType = entityType,
                    EntityId = GetEntityId(entry, useCurrentValues: true),
                    Action = AuditAction.Create,
                    UserId = _user.Id,
                    TimestampUtc = utcNow,
                    NewValues = SerializeValues(ExcludeAuditableStamps(entry.Properties), current: true),
                    References = BuildReferences(entry, entityType, useCurrentValues: true)
                };

            case EntityState.Deleted:
                return new AuditTrail
                {
                    EntityType = entityType,
                    EntityId = GetEntityId(entry, useCurrentValues: false),
                    Action = AuditAction.Delete,
                    UserId = _user.Id,
                    TimestampUtc = utcNow,
                    OldValues = SerializeValues(ExcludeAuditableStamps(entry.Properties), current: false),
                    References = BuildReferences(entry, entityType, useCurrentValues: false)
                };

            case EntityState.Modified:
                var modified = ExcludeAuditableStamps(entry.Properties)
                    .Where(p => p.IsModified && !Equals(p.CurrentValue, p.OriginalValue))
                    .ToList();

                if (modified.Count == 0)
                {
                    return null;
                }

                return new AuditTrail
                {
                    EntityType = entityType,
                    EntityId = GetEntityId(entry, useCurrentValues: true),
                    Action = AuditAction.Update,
                    UserId = _user.Id,
                    TimestampUtc = utcNow,
                    OldValues = SerializeValues(modified, current: false),
                    NewValues = SerializeValues(modified, current: true),
                    AffectedColumns = JsonSerializer.Serialize(modified.Select(p => p.Metadata.Name)),
                    References = BuildReferences(entry, entityType, useCurrentValues: true)
                };

            default:
                return null;
        }
    }

    private static IEnumerable<PropertyEntry> ExcludeAuditableStamps(IEnumerable<PropertyEntry> properties) =>
        properties.Where(p => !_excludedProperties.Contains(p.Metadata.Name));

    private static bool IsAudited(EntityEntry entry) =>
        entry.Entity is IAudited || _auditedSharedTypeEntities.Contains(GetEntityTypeName(entry));

    // EF Core gives shared-type (property-bag) entities, like our join tables, the short name
    // they were registered with; ordinary CLR entities get their full namespace-qualified name
    // from Metadata.Name, so use ClrType.Name instead to get e.g. "Company" not "Core.Shared.Company".
    private static string GetEntityTypeName(EntityEntry entry) =>
        entry.Metadata.IsPropertyBag ? entry.Metadata.Name : entry.Metadata.ClrType.Name;

    private static string GetEntityId(EntityEntry entry, bool useCurrentValues)
    {
        var keyProperties = entry.Metadata.FindPrimaryKey()!.Properties;

        var values = keyProperties.Select(p =>
        {
            var propertyEntry = entry.Property(p.Name);
            var value = useCurrentValues ? propertyEntry.CurrentValue : propertyEntry.OriginalValue;
            return value?.ToString() ?? string.Empty;
        });

        return string.Join(":", values);
    }

    private static List<AuditTrailReference> BuildReferences(EntityEntry entry, string entityType, bool useCurrentValues)
    {
        if (!ReferenceMap.TryGetValue(entityType, out var mappings))
        {
            return [];
        }

        var references = new List<AuditTrailReference>();

        foreach (var (fkProperty, referencedType) in mappings)
        {
            var propertyEntry = entry.Property(fkProperty);
            var value = useCurrentValues ? propertyEntry.CurrentValue : propertyEntry.OriginalValue;

            if (value is null)
            {
                continue;
            }

            references.Add(new AuditTrailReference
            {
                ReferencedEntityType = referencedType,
                ReferencedEntityId = value.ToString()!
            });
        }

        return references;
    }

    private static string SerializeValues(IEnumerable<PropertyEntry> properties, bool current)
    {
        var values = properties.ToDictionary(
            p => p.Metadata.Name,
            p => current ? p.CurrentValue : p.OriginalValue);

        return JsonSerializer.Serialize(values);
    }
}
