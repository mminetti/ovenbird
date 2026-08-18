using Core.Security;

namespace IntegrationTests.Data;

public class EfRepositoryDelete : BaseEfRepoTestFixture
{
    [Fact]
    public async Task DeletesItemAfterAddingIt()
    {
        // add a user
        var repository = GetRepository();

        var user = new User
        {
            ExternalIdentifier = Guid.NewGuid().ToString(),
            Name = "User",
            Email = "test@test.com",
            IsActive = true
        };

        await repository.AddAsync(user, CancellationToken.None);

        // delete the item
        await repository.DeleteAsync(user, CancellationToken.None);

        // verify it's no longer there
        (await repository.ListAsync(CancellationToken.None)).ShouldNotContain(x => x.Name == user.Name);
    }
}
