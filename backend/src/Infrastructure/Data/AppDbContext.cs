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
    public DbSet<Connector> Connector => Set<Connector>();
    public DbSet<ConnectorField> ConnectorField => Set<ConnectorField>();
    public DbSet<ConnectorImplementation> ConnectorImplementation => Set<ConnectorImplementation>();
    public DbSet<Configuration> Configuration => Set<Configuration>();
    public DbSet<ConfigurationField> ConfigurationField => Set<ConfigurationField>();
    public DbSet<ConfigurationType> ConfigurationType => Set<ConfigurationType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges() =>
          SaveChangesAsync().GetAwaiter().GetResult();
}
