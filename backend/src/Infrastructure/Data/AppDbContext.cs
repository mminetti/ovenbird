using Core.ContributorAggregate;
using Core.Security;

namespace Infrastructure.Data;

// Package Manager Console
// dotnet ef migrations add _MIGRATION_NAME_ --startup-project "src\Web" --project "src\Infrastructure" --output-dir Data/Migrations
// dotnet ef migrations script _MIGRATION_NAME_FROM_ --idempotent --context "AppDbContext" --startup-project "src\Web" --project "src\Infrastructure"

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contributor> Contributors => Set<Contributor>();
    public DbSet<User> User => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges() =>
          SaveChangesAsync().GetAwaiter().GetResult();
}
