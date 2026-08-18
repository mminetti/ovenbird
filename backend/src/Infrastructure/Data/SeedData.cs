using Core.Security;

namespace Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        if (await dbContext.User.AnyAsync()) return; // DB has been seeded

        await PopulateTestDataAsync(dbContext);
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
