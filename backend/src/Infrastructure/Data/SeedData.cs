using Core.Security;

namespace Infrastructure.Data;

public static class SeedData
{
    private const string AdminRoleName = "Admin";

    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await EnsureSecurityDataAsync(dbContext);

        if (await dbContext.User.AnyAsync()) return; // DB has been seeded

        await PopulateTestDataAsync(dbContext);
    }

    public static async Task EnsureSecurityDataAsync(AppDbContext dbContext)
    {
        if (await dbContext.Role.AnyAsync()) return; // Security data has been seeded

        var permissions = await dbContext.Permission.ToListAsync();

        var adminRole = new Role { Name = AdminRoleName };
        adminRole.SetPermissions(permissions);

        dbContext.Role.Add(adminRole);

        await dbContext.SaveChangesAsync();
    }

    public static async Task PopulateTestDataAsync(AppDbContext dbContext)
    {
        dbContext.User.Add(new User
        {
            ExternalIdentifier = "ext-1",
            Name = "Seed User 1",
            Email = "user1@example.com",
            IsActive = true
        });

        await dbContext.SaveChangesAsync();
    }
}
