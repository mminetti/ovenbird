# Plan: Generic audit trail for entities

## Context

The app needs a common "who changed what, when, from what to what" audit trail across most domain entities (Market, Company, Configuration, Connector, User, Role, Permission, etc. — 17 entities today, more will be added over time). Today `AuditableEntityBase`/`IAuditableEntity` (`src/Core/Common/AuditableEntityBase.cs`, `IAuditableEntity.cs`) only stamp `CreatedBy`/`CreatedAtUtc`/`LastModifiedBy`/`LastModifiedAtUtc` on the entity itself — there's no history of individual changes, no record of deletes, and no way to see old vs. new values. There's also no way today to see "everything that happened to Configuration X", including changes to its child `ConfigurationField` rows or to many-to-many membership (e.g. a `Permission` added/removed from a `Role`).

Two `SaveChangesInterceptor`s already exist and are the natural extension point: `AuditableEntityInterceptor` (stamps timestamps) and `EventDispatchInterceptor` (dispatches domain events post-save), both registered in `src/Infrastructure/InfrastructureServiceExtensions.cs`. This plan adds a third interceptor that generically diffs every `SaveChanges` call and writes structured audit rows for the entities that opt in — a new entity is *not* audited automatically just by being added to `AppDbContext`; it must implement `IAudited` (or, for join rows, be added to the interceptor's explicit name list).

Decisions confirmed with the user:
- **Storage shape**: one row per changed entity (Create/Update/Delete), with old/new values stored as JSON columns — not one row per field.
- **Scope**: opt-in, not all entities tracked by `AppDbContext.ChangeTracker`. A new marker interface `IAudited` (`src/Core/Common/IAudited.cs`) is added; only entities that implement it get audit rows. This is deliberately separate from `IAuditableEntity`/`AuditableEntityBase` (which only stamps `CreatedBy`/`LastModifiedBy`) — an entity can have one, both, or neither.
- **Entities that implement `IAudited`**: `Configuration`, `Connector`, `Company`, `Market`, `User`, `Role`, `Permission`. (Not audited: `MarketDocument`, plus anything else not in this list.)
- **Rollups opt in separately too, and are enabled for all of these**:
  - Child entities: `ConfigurationField` (rolls into `Configuration`) and `ConnectorField` (rolls into `Connector`) both implement `IAudited` themselves, with `ReferenceMap` pointing back to their parent.
  - Join tables: `UserRole` (`User`↔`Role`) and `RolePermission` (`Role`↔`Permission`) are both added to the interceptor's `AuditedSharedTypeEntities` name list, since join rows are EF shared-type entities with no CLR class to implement `IAudited` on.
- **"Reference"**: two things — (1) many-to-many membership changes (`UserRole`, `RolePermission` join rows) recorded as their own audit events, and (2) child-entity changes (e.g. `ConfigurationField`) rolled up so they're visible when browsing the parent's (`Configuration`) history. Both are handled by one generic mechanism (see below), gated by the opt-in above.
- **Scope of this change**: backend only — capture + a query API. No frontend viewer yet.

## Design

### Schema — two new tables, both under `Core/Common`

`AuditTrail` (one row per changed entity per save):
- `Id` (long, identity, PK)
- `EntityType` (string) — CLR/EF entity name, e.g. `"Market"`, `"UserRole"`
- `EntityId` (string) — the entity's PK, stringified so it works for any `TId`; for shared-type join entities (composite key) it's a formatted composite string
- `Action` (enum: `Create`, `Update`, `Delete`)
- `UserId` (string?) — from `IUser.Id`, same source `AuditableEntityInterceptor` already uses
- `TimestampUtc` (`DateTimeOffset`)
- `OldValues` (string?, JSON) — null on Create
- `NewValues` (string?, JSON) — null on Delete
- `AffectedColumns` (string?, JSON array of property names changed — only meaningful for Update)
- `References` (owned collection, see below)

`AuditTrailReference` (owned by `AuditTrail`, no independent identity):
- `ReferencedEntityType` (string)
- `ReferencedEntityId` (string)

A single `AuditTrail` row can carry zero or more references. This one mechanism covers both required cases:
- **Join-table changes** (`UserRole` Added/Deleted): references = `[("User", userId), ("Role", roleId)]` — both sides of the relationship.
- **Child rollup** (`ConfigurationField` Added/Modified/Deleted): references = `[("Configuration", configurationId)]` — the parent aggregate.

Querying "history of Configuration X" = `AuditTrail` where (`EntityType == "Configuration" && EntityId == X`) OR it has a reference `("Configuration", X)`.

### Capture mechanism — opt-in check + declarative map + generic interceptor

Two small static declarations in the interceptor:

```csharp
// Shared-type (join) entities have no CLR class to implement IAudited on,
// so their opt-in is this explicit name list instead.
private static readonly HashSet<string> AuditedSharedTypeEntities = new()
{
    "UserRole",
    "RolePermission",
};

private static bool IsAudited(EntityEntry entry) =>
    entry.Entity is IAudited || AuditedSharedTypeEntities.Contains(entry.Metadata.Name);
```

```csharp
private static readonly Dictionary<string, (string FkProperty, string ReferencedType)[]> ReferenceMap = new()
{
    ["UserRole"] = [("UserId", "User"), ("RoleId", "Role")],
    ["RolePermission"] = [("RoleId", "Role"), ("PermissionId", "Permission")],
    [nameof(ConfigurationField)] = [("ConfigurationId", nameof(Configuration))],
    [nameof(ConnectorField)] = [("ConnectorId", nameof(Connector))],
    // extend as new child/join relationships are added
};
```

`ReferenceMap` only matters for entries that already pass `IsAudited` — it says which FK-referenced entities to record alongside an already-in-scope row, it does not itself bring an entity into scope.

`"UserRole"`/`"RolePermission"` are EF shared-type entities (`Dictionary<string, object>`) configured via `UsingEntity<Dictionary<string,object>>(...)` in `UserConfiguration.cs`/`RoleConfiguration.cs` — confirmed these DO show up as their own `EntityEntry` in `ChangeTracker.Entries()` with `Added`/`Deleted` states when collection membership changes, so no special-casing beyond `AuditedSharedTypeEntities` is needed; the generic entry loop picks them up like any other entity once their name is added to that set.

New interceptor: `src/Infrastructure/Data/Interceptors/AuditTrailInterceptor.cs`, implementing `SaveChangesInterceptor`, mirroring the existing two-phase pattern already used in this codebase:

- `SavingChangesAsync` (pre-save, same hook `AuditableEntityInterceptor` uses):
  - Loop `context.ChangeTracker.Entries()` (non-generic — must include shared-type join entities), skip `Unchanged`/`Detached`, skip the `AuditTrail`/`AuditTrailReference` types themselves (avoid recursion), and skip any entry where `IsAudited(entry)` is false.
  - For `Added`: Action = Create, `NewValues` = all scalar/FK properties from `CurrentValues`, `OldValues` = null.
  - For `Deleted`: Action = Delete, `OldValues` = all scalar/FK properties from `OriginalValues`, `NewValues` = null.
  - For `Modified`: only proceed if `entry.Properties.Any(p => p.IsModified)`; Action = Update, `AffectedColumns` = modified property names, `OldValues`/`NewValues` = original/current values for just those properties.
  - Resolve `References` via `ReferenceMap[entry.Metadata.Name]` using the relevant value source (CurrentValues for Create/Update, OriginalValues for Delete).
  - For `Added` entries, the real PK isn't generated yet — build the `AuditTrail` row with a placeholder `EntityId` and keep a side-list of `(AuditTrail row, EntityEntry sourceEntry)` pairs to fix up after save.
  - Add all built `AuditTrail` rows via `context.Set<AuditTrail>().AddRange(...)` before returning — EF picks these up as part of the same `SaveChanges` call since this runs before command generation (same trick already proven by `AuditableEntityInterceptor` mutating tracked entities in this hook).
- `SavedChangesAsync` (post-save, same hook `EventDispatchInterceptor` uses): for the side-list of Created entries, now that `SaveChanges` has run and generated keys are populated, set the real `EntityId` (and any reference id that depended on a just-generated key, e.g. a self-referencing Added join row) and issue one more `SaveChangesAsync` to persist just those corrected rows.

Reuses `IUser` (`src/UseCases/Common/IUser.cs`) for `UserId` — same abstraction `AuditableEntityInterceptor` already depends on, so behavior (`"system"` in Worker via `SystemUser`, authenticated user's email in Web via `HttpUser`) is consistent with existing `CreatedBy`/`LastModifiedBy` stamping.

Register in `src/Infrastructure/InfrastructureServiceExtensions.cs` alongside the existing two interceptors:
```csharp
services.AddScoped<AuditTrailInterceptor>();
...
options.AddInterceptors(eventDispatchInterceptor, auditableEntityInterceptor, auditTrailInterceptor);
```

### EF configuration

- `src/Infrastructure/Data/Config/Common/AuditTrailConfiguration.cs`: `IEntityTypeConfiguration<AuditTrail>`, table `AuditTrails`, `.OwnsMany(x => x.References, ...)` mapped to a child table `AuditTrailReferences` with FK back to `AuditTrail.Id` — no separate `DbSet`/repository needed for the owned type. Index on `(EntityType, EntityId)` and a separate index on the owned table's `(ReferencedEntityType, ReferencedEntityId)` for the rollup query. `OldValues`/`NewValues`/`AffectedColumns` mapped as `nvarchar(max)`.
- Add `DbSet<AuditTrail> AuditTrails` to `src/Infrastructure/Data/AppDbContext.cs` (and to `ReadDbContext.cs` for the query side).
- New migration: `dotnet ef migrations add AddAuditTrail --startup-project "src\Web" --project "src\Infrastructure" --output-dir Data/Migrations --context "AppDbContext"` (per the command already documented at the top of `AppDbContext.cs`).

### Query API (backend only, no UI this round)

Follows the existing list-query pattern (`src/UseCases/Companies/List/*`, `src/Infrastructure/Data/Queries/Shared/ListCompaniesQueryService.cs`, `IListCompaniesQueryService`):

- `src/UseCases/Auditing/AuditTrailDto.cs`, `src/UseCases/Auditing/List/ListAuditTrailQuery.cs` (filters: `EntityType`, `EntityId`, paging), `ListAuditTrailHandler.cs`, `IListAuditTrailQueryService.cs`.
- `src/Infrastructure/Data/Queries/Common/ListAuditTrailQueryService.cs`: queries `ReadDbContext.AuditTrails` for rows matching `EntityType`/`EntityId` **or** having a matching owned `References` row — this is what surfaces child/join rollups.
- Register the query service in `InfrastructureServiceExtensions.cs` next to the other `IListXQueryService` registrations.
- `src/Web/Auditing/List/ListAuditTrail.cs`: FastEndpoints `Endpoint<ListAuditTrailRequest, ...>`, `Get("/audit-trail")`, invoking `bus.InvokeAsync<Result<ItemPagedResult<AuditTrailDto>>>(new ListAuditTrailQuery(...), ct)` — same shape as `src/Web/Companies/*` endpoints. Gate with a `Permissions(...)` call consistent with other list endpoints (check what permission other admin-ish list endpoints use, e.g. Roles/Permissions list, and reuse/extend that convention).

## Files

**New:**
- `src/Core/Common/AuditTrail.cs`, `AuditTrailReference.cs`, `AuditAction.cs` (enum)
- `src/Core/Common/IAudited.cs` — marker interface, implemented by `Configuration`, `Connector`, `Company`, `Market`, `User`, `Role`, `Permission`, `ConfigurationField`, `ConnectorField`
- `src/Infrastructure/Data/Interceptors/AuditTrailInterceptor.cs`
- `src/Infrastructure/Data/Config/Common/AuditTrailConfiguration.cs`
- `src/UseCases/Auditing/AuditTrailDto.cs`, `List/ListAuditTrailQuery.cs`, `List/ListAuditTrailHandler.cs`, `List/IListAuditTrailQueryService.cs`
- `src/Infrastructure/Data/Queries/Common/ListAuditTrailQueryService.cs`
- `src/Web/Auditing/List/ListAuditTrail.cs` (+ Request/Response records)
- New EF migration under `src/Infrastructure/Data/Migrations/`
- `tests/IntegrationTests/Data/AuditTrailInterceptorTests.cs`

**Changed:**
- `src/Infrastructure/InfrastructureServiceExtensions.cs` — register interceptor + query service
- `src/Infrastructure/Data/AppDbContext.cs`, `ReadDbContext.cs` — add `DbSet<AuditTrail>`

## Testing

Extend the existing fixture pattern in `tests/IntegrationTests/Data/BaseEfRepoTestFixture.cs` (EF Core InMemory `AppDbContext`) to also register `AuditTrailInterceptor`, following how it already wires `EventDispatchInterceptor`. Add `AuditTrailInterceptorTests.cs` covering:
- Create/Update/Delete of `Company` (a plain `IAudited` entity, no rollups involved) produces one correctly-shaped `AuditTrail` row with the right `Action`, `OldValues`/`NewValues`/`AffectedColumns`, and the real generated `EntityId` after the post-save fixup.
- Create/Update/Delete of `MarketDocument` (extends `AuditableEntityBase` but does **not** implement `IAudited`) produces no `AuditTrail` row — confirms the two concerns are independent.
- Adding/removing a `Role` from a `User.Roles` collection produces a `UserRole` audit row (via `AuditedSharedTypeEntities`) with both `User` and `Role` as references; same for adding/removing a `Permission` from a `Role.Permissions` collection producing a `RolePermission` row.
- Adding/updating a `ConfigurationField` under a `Configuration` produces an audit row whose reference is that `Configuration`, and that `ListAuditTrailQueryService` returns it when queried by `(Configuration, configurationId)` — even when `Configuration` itself is not directly modified in that save. Same for `ConnectorField`/`Connector`.

## Verification

1. `cd backend && dotnet ef migrations add AddAuditTrail --startup-project "src\Web" --project "src\Infrastructure" --output-dir Data/Migrations --context "AppDbContext"` and inspect the generated migration for correctness.
2. `cd backend && dotnet build Ovenbird.slnx` (required by CLAUDE.md before considering backend work done).
3. Run the new and existing integration tests: `cd backend && dotnet test tests/IntegrationTests`.
4. Manually exercise via the API (e.g. update a `Company`, add/remove a `Role` from a `User`, add a `ConfigurationField`) and call the new `GET /audit-trail` endpoint to confirm rows appear, including rollups.
