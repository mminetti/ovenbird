using Core.Security;

namespace IntegrationTests.Data;

public class EfRepositoryUpdate : BaseEfRepoTestFixture
{
    [Fact]
    public async Task UpdatesItemAfterAddingIt()
    {
        var repository = GetRepository();

        var user = new User
        {
            ExternalIdentifier = Guid.NewGuid().ToString(),
            Name = "User",
            Email = "test@test.com",
            IsActive = true
        };

        await repository.AddAsync(user, CancellationToken.None);

        // detach the item so we get a different instance
        _dbContext.Entry(user).State = EntityState.Detached;

        // fetch the item and update its name
        var newUser = (await repository.ListAsync(CancellationToken.None))
            .FirstOrDefault(x => x.Name == user.Name);
        newUser.ShouldNotBeNull();

        user.ShouldNotBeSameAs(newUser);
        var newName = Guid.NewGuid().ToString();
        newUser.UpdateName(newName);

        // Update the item
        await repository.UpdateAsync(newUser, CancellationToken.None);

        // Fetch the updated item
        var updatedItem = (await repository.ListAsync(CancellationToken.None))
            .FirstOrDefault(Contributor => Contributor.Name == newName);

        updatedItem.ShouldNotBeNull();
        user.Name.ShouldNotBe(updatedItem.Name);
        user.Email.ShouldBe(updatedItem.Email);
        newUser.Id.ShouldBe(updatedItem.Id);
    }
}
