# Plan: Generic audit trail for entities

## Context

The app needs a common "who changed what, when, from what to what" audit trail across most domain entities (Market, Company, Configuration, Connector, User, Role, Permission, etc. — 17 entities today, more will be added over time). Today `AuditableEntityBase`/`IAuditableEntity` (`src/Core/Common/AuditableEntityBase.cs`, `IAuditableEntity.cs`) only stamp `CreatedBy`/`CreatedAtUtc`/`LastModifiedBy`/`LastModifiedAtUtc` on the entity itself — there's no history of individual changes, no record of deletes, and no way to see old vs. new values. There's also no way today to see "everything that happened to Configuration X", including changes to its child `ConfigurationField` rows or to many-to-many membership (e.g. a `Permission` added/removed from a `Role`).

Two `SaveChangesInterceptor`s already exist and are the natural extension point: `AuditableEntityInterceptor` (stamps timestamps) and `EventDispatchInterceptor` (dispatches domain events post-save), both registered in `src/Infrastructure/InfrastructureServiceExtensions.cs`. This plan adds a third interceptor that generically diffs every `SaveChanges` call and writes structured audit rows, with no per-entity or per-handler code required — new entities get audited automatically as soon as they're added to `AppDbContext`.

Decisions confirmed with the user:
- **Storage shape**: one row per changed entity (Create/Update/Delete), with old/new values stored as JSON columns — not one row per field.
- **Scope**: all entities tracked by `AppDbContext.ChangeTracker`, not just the 9 that implement `IAuditableEntity` today.
- **"Related entity"**: two things — (1) many-to-many membership changes (`UserRole`, `RolePermission` join rows) recorded as their own audit events, and (2) child-entity changes (e.g. `ConfigurationField`) rolled up so they're visible when browsing the parent's (`Configuration`) history. Both are handled by one generic mechanism (see below).
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
- `RelatedEntities` (owned collection, see below)

`AuditTrailRelatedEntity` (owned by `AuditTrail`, no independent identity):
- `RelatedEntityType` (string)
- `RelatedEntityId` (string)

A single `AuditTrail` row can carry zero or more related entities. This one mechanism covers both required cases:
- **Join-table changes** (`UserRole` Added/Deleted): related entities = `[("User", userId), ("Role", roleId)]` — both sides of the relationship.
- **Child rollup** (`ConfigurationField` Added/Modified/Deleted): related entities = `[("Configuration", configurationId)]` — the parent aggregate.

Querying "history of Configuration X" = `AuditTrail` where (`EntityType == "Configuration" && EntityId == X`) OR it has a related entity `("Configuration", X)`.

### Capture mechanism — declarative map + generic interceptor

A small static map in the interceptor declares, per entity-type name, which FK properties point to entities worth recording as "related":

```csharp
private static readonly Dictionary<string, (string FkProperty, string RelatedType)[]> RelatedEntityMap = new()
{
    ["UserRole"] = [("UserId", "User"), ("RoleId", "Role")],
    ["RolePermission"] = [("RoleId", "Role"), ("PermissionId", "Permission")],
    [nameof(ConfigurationField)] = [("ConfigurationId", nameof(Configuration))],
    [nameof(ConnectorField)] = [("ConnectorId", nameof(Connector))],
    // extend as new child/join relationships are added
};
```

`"UserRole"`/`"RolePermission"` are EF shared-type entities (`Dictionary<string, object>`) configured via `UsingEntity<Dictionary<string,object>>(...)` in `UserConfiguration.cs`/`RoleConfiguration.cs` — confirmed these DO show up as their own `EntityEntry` in `ChangeTracker.Entries()` with `Added`/`Deleted` states when collection membership changes, so no special-casing beyond this map is needed; the generic entry loop picks them up like any other entity.

New interceptor: `src/Infrastructure/Data/Interceptors/AuditTrailInterceptor.cs`, implementing `SaveChangesInterceptor`, mirroring the existing two-phase pattern already used in this codebase:

- `SavingChangesAsync` (pre-save, same hook `AuditableEntityInterceptor` uses):
  - Loop `context.ChangeTracker.Entries()` (non-generic — must include shared-type join entities), skip `Unchanged`/`Detached`, and skip the `AuditTrail`/`AuditTrailRelatedEntity` types themselves (avoid recursion).
  - For `Added`: Action = Create, `NewValues` = all scalar/FK properties from `CurrentValues`, `OldValues` = null.
  - For `Deleted`: Action = Delete, `OldValues` = all scalar/FK properties from `OriginalValues`, `NewValues` = null.
  - For `Modified`: only proceed if `entry.Properties.Any(p => p.IsModified)`; Action = Update, `AffectedColumns` = modified property names, `OldValues`/`NewValues` = original/current values for just those properties.
  - Resolve `RelatedEntities` via `RelatedEntityMap[entry.Metadata.Name]` using the relevant value source (CurrentValues for Create/Update, OriginalValues for Delete).
  - For `Added` entries, the real PK isn't generated yet — build the `AuditTrail` row with a placeholder `EntityId` and keep a side-list of `(AuditTrail row, EntityEntry sourceEntry)` pairs to fix up after save.
  - Add all built `AuditTrail` rows via `context.Set<AuditTrail>().AddRange(...)` before returning — EF picks these up as part of the same `SaveChanges` call since this runs before command generation (same trick already proven by `AuditableEntityInterceptor` mutating tracked entities in this hook).
- `SavedChangesAsync` (post-save, same hook `EventDispatchInterceptor` uses): for the side-list of Created entries, now that `SaveChanges` has run and generated keys are populated, set the real `EntityId` (and any related-entity id that depended on a just-generated key, e.g. a self-referencing Added join row) and issue one more `SaveChangesAsync` to persist just those corrected rows.

Reuses `IUser` (`src/UseCases/Common/IUser.cs`) for `UserId` — same abstraction `AuditableEntityInterceptor` already depends on, so behavior (`"system"` in Worker via `SystemUser`, authenticated user's email in Web via `HttpUser`) is consistent with existing `CreatedBy`/`LastModifiedBy` stamping.

Register in `src/Infrastructure/InfrastructureServiceExtensions.cs` alongside the existing two interceptors:
```csharp
services.AddScoped<AuditTrailInterceptor>();
...
options.AddInterceptors(eventDispatchInterceptor, auditableEntityInterceptor, auditTrailInterceptor);
```

### EF configuration

- `src/Infrastructure/Data/Config/Common/AuditTrailConfiguration.cs`: `IEntityTypeConfiguration<AuditTrail>`, table `AuditTrails`, `.OwnsMany(x => x.RelatedEntities, ...)` mapped to a child table `AuditTrailRelatedEntities` with FK back to `AuditTrail.Id` — no separate `DbSet`/repository needed for the owned type. Index on `(EntityType, EntityId)` and a separate index on the owned table's `(RelatedEntityType, RelatedEntityId)` for the rollup query. `OldValues`/`NewValues`/`AffectedColumns` mapped as `nvarchar(max)`.
- Add `DbSet<AuditTrail> AuditTrails` to `src/Infrastructure/Data/AppDbContext.cs` (and to `ReadDbContext.cs` for the query side).
- New migration: `dotnet ef migrations add AddAuditTrail --startup-project "src\Web" --project "src\Infrastructure" --output-dir Data/Migrations --context "AppDbContext"` (per the command already documented at the top of `AppDbContext.cs`).

### Query API (backend only, no UI this round)

Follows the existing list-query pattern (`src/UseCases/Companies/List/*`, `src/Infrastructure/Data/Queries/Shared/ListCompaniesQueryService.cs`, `IListCompaniesQueryService`):

- `src/UseCases/Auditing/AuditTrailDto.cs`, `src/UseCases/Auditing/List/ListAuditTrailQuery.cs` (filters: `EntityType`, `EntityId`, paging), `ListAuditTrailHandler.cs`, `IListAuditTrailQueryService.cs`.
- `src/Infrastructure/Data/Queries/Common/ListAuditTrailQueryService.cs`: queries `ReadDbContext.AuditTrails` for rows matching `EntityType`/`EntityId` **or** having a matching owned `RelatedEntities` row — this is what surfaces child/join rollups.
- Register the query service in `InfrastructureServiceExtensions.cs` next to the other `IListXQueryService` registrations.
- `src/Web/Auditing/List/ListAuditTrail.cs`: FastEndpoints `Endpoint<ListAuditTrailRequest, ...>`, `Get("/audit-trail")`, invoking `bus.InvokeAsync<Result<ItemPagedResult<AuditTrailDto>>>(new ListAuditTrailQuery(...), ct)` — same shape as `src/Web/Companies/*` endpoints. Gate with a `Permissions(...)` call consistent with other list endpoints (check what permission other admin-ish list endpoints use, e.g. Roles/Permissions list, and reuse/extend that convention).

## Files

**New:**
- `src/Core/Common/AuditTrail.cs`, `AuditTrailRelatedEntity.cs`, `AuditAction.cs` (enum)
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
- Create/Update/Delete of a plain entity (e.g. `Company`) produces one correctly-shaped `AuditTrail` row with the right `Action`, `OldValues`/`NewValues`/`AffectedColumns`, and the real generated `EntityId` after the post-save fixup.
- Adding/removing a `Role` from a `User.Roles` collection produces a `UserRole` audit row with both `User` and `Role` as related entities.
- Adding/updating a `ConfigurationField` under a `Configuration` produces an audit row whose related entity is that `Configuration`, and that `ListAuditTrailQueryService` returns it when queried by `(Configuration, configurationId)`.

## Verification

1. `cd backend && dotnet ef migrations add AddAuditTrail --startup-project "src\Web" --project "src\Infrastructure" --output-dir Data/Migrations --context "AppDbContext"` and inspect the generated migration for correctness.
2. `cd backend && dotnet build Ovenbird.slnx` (required by CLAUDE.md before considering backend work done).
3. Run the new and existing integration tests: `cd backend && dotnet test tests/IntegrationTests`.
4. Manually exercise via the API (e.g. update a `Company`, add/remove a `Role` from a `User`, add a `ConfigurationField`) and call the new `GET /audit-trail` endpoint to confirm rows appear, including rollups.
