using Core.ContributorAggregate;
using Core.Security;

namespace Infrastructure.Data;

public static class SeedData
{
    public const int NUMBER_OF_CONTRIBUTORS = 30;

    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        if (await dbContext.Contributors.AnyAsync()) return; // DB has been seeded

        await PopulateTestDataAsync(dbContext);
    }

    public static async Task PopulateTestDataAsync(AppDbContext dbContext)
    {
        // add a bunch more contributors to support demonstrating paging
        for (int i = 1; i <= NUMBER_OF_CONTRIBUTORS; i++)
        {
            dbContext.Contributors.Add(new Contributor($"Contributor {i}"));
        }

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
