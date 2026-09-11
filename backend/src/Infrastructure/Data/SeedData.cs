using System.Reflection;
using Core.Security;
using UseCases.Common;

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
        if (await dbContext.Permission.AnyAsync()) return; // Security data has been seeded

        var permissions = typeof(Constants.Permissions)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => new Permission { Name = (string)f.GetValue(null)! })
            .ToList();

        var adminRole = new Role { Name = AdminRoleName };
        adminRole.SetPermissions(permissions);

        dbContext.Permission.AddRange(permissions);
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
