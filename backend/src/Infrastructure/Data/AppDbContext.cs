using Core.Market;
using Core.Security;
using Core.Shared;

namespace Infrastructure.Data;

// Package Manager Console
// dotnet ef migrations add _MIGRATION_NAME_ --startup-project "src\Web" --project "src\Infrastructure" --output-dir Data/Migrations --context "AppDbContext"
// dotnet ef migrations script _MIGRATION_NAME_FROM_ --idempotent --context "AppDbContext" --startup-project "src\Web" --project "src\Infrastructure"

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> User => Set<User>();
    public DbSet<Role> Role => Set<Role>();
    public DbSet<Permission> Permission => Set<Permission>();
    public DbSet<Core.Security.Module> Module => Set<Core.Security.Module>();
    public DbSet<Company> Company => Set<Company>();
    public DbSet<MarketDocument> MarketDocument => Set<MarketDocument>();
    public DbSet<Integration> SystemIntegration => Set<Integration>();
    public DbSet<IntegrationField> SystemIntegrationField => Set<IntegrationField>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges() =>
          SaveChangesAsync().GetAwaiter().GetResult();
}
