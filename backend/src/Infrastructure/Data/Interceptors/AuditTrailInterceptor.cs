using System.Text.Json;
using Core.Common;
using Core.Security;
using Core.Settings;
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
        "RolePermission",
        "ConfigurationConnector"
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
        ["ConfigurationConnector"] = [("ConfigurationId", nameof(Configuration)), ("ConnectorId", nameof(Connector))],
        [nameof(ConfigurationField)] = [("ConfigurationId", nameof(Configuration))],
        [nameof(ConnectorField)] = [("ConnectorId", nameof(Connector))]
    };

    private readonly TimeProvider _dateTime = dateTime;
    private readonly IUser _user = user;
    private readonly List<(AuditTrail Row, EntityEntry Entry)> _pendingCreates = [];

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            await CaptureAuditTrail(context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
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
                row.NewValues = await SerializeValues(
                    context, entry, ExcludeAuditableStamps(entry.Properties), current: true, cancellationToken);
                row.References = BuildReferences(entry, GetEntityTypeName(entry), useCurrentValues: true);
            }

            _pendingCreates.Clear();

            await context.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task CaptureAuditTrail(DbContext context, CancellationToken cancellationToken)
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

            var row = await BuildAuditRow(context, entry, utcNow, cancellationToken);

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

    private async Task<AuditTrail?> BuildAuditRow(
        DbContext context, EntityEntry entry, DateTimeOffset utcNow, CancellationToken cancellationToken)
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
                    NewValues = await SerializeValues(
                        context, entry, ExcludeAuditableStamps(entry.Properties), current: true, cancellationToken),
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
                    OldValues = await SerializeValues(
                        context, entry, ExcludeAuditableStamps(entry.Properties), current: false, cancellationToken),
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
                    OldValues = await SerializeValues(context, entry, modified, current: false, cancellationToken),
                    NewValues = await SerializeValues(context, entry, modified, current: true, cancellationToken),
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
    // from Metadata.Name, so use ClrType.Name instead to get e.g. "Company" not "Core.Settings.Company".
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

    private static async Task<string> SerializeValues(
        DbContext context, EntityEntry entry, IEnumerable<PropertyEntry> properties, bool current, CancellationToken cancellationToken)
    {
        var values = properties.ToDictionary(
            p => p.Metadata.Name,
            p => current ? p.CurrentValue : p.OriginalValue);

        foreach (var (fkProperty, principalType, labelProperty) in DiscoverLabelableFks(entry))
        {
            if (!values.TryGetValue(fkProperty, out var id) || id is null)
            {
                continue;
            }

            values[labelProperty] = await ResolveNameAsync(context, principalType, id, cancellationToken);
        }

        return JsonSerializer.Serialize(values);
    }

    // Discovers labelable FKs from EF's own model metadata rather than a hand-maintained map,
    // so a new audited entity's foreign keys are picked up automatically - no interceptor edit
    // needed, only the referenced type opting in via IHasDisplayName (see ResolveNameAsync).
    private static IEnumerable<(string FkProperty, Type PrincipalType, string LabelProperty)> DiscoverLabelableFks(EntityEntry entry)
    {
        foreach (var fk in entry.Metadata.GetForeignKeys())
        {
            if (fk.Properties.Count != 1)
            {
                continue;
            }

            var fkProperty = fk.Properties[0].Name;

            if (!fkProperty.EndsWith("Id", StringComparison.Ordinal) || fkProperty.Length == 2)
            {
                continue;
            }

            var principalType = fk.PrincipalEntityType.ClrType;

            if (!typeof(IHasDisplayName).IsAssignableFrom(principalType))
            {
                continue;
            }

            yield return (fkProperty, principalType, fkProperty[..^2] + "Name");
        }
    }

    // FindAsync checks the change tracker's locally-tracked entities (even ones with a
    // temporary key, e.g. a new parent created in the same SaveChanges call) before it ever
    // queries the database.
    private static async Task<string?> ResolveNameAsync(DbContext context, Type principalType, object id, CancellationToken cancellationToken) =>
        (await context.FindAsync(principalType, [id], cancellationToken) as IHasDisplayName)?.Name;
}
