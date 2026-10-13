# Ovenbird

## Verification before declaring work done

- After any change under `frontend/`, run lint before saying the task is done:
  `cd frontend && pnpm lint`
- After any change under `backend/`, build the whole solution before saying the task is done:
  `cd backend && dotnet build Ovenbird.slnx`

Fix any errors these report before considering the change complete.

## EF Core migrations

Whenever you add an EF Core migration (`dotnet ef migrations add ...` from `backend/src/Web`), also generate its idempotent SQL script and commit it alongside the migration:

```bash
cd backend/src/Web
dotnet ef migrations script <PreviousMigrationName> --idempotent --context AppDbContext -p ../Infrastructure/Infrastructure.csproj -s Web.csproj -o ../../scripts/<NewMigrationTimestamp_Name>.sql
```

`<PreviousMigrationName>` is the migration immediately before the new one (omit it to script from scratch). Name the `.sql` file after the new migration.
