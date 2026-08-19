using Core.Security;

namespace IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
    [Fact]
    public async Task AddsUserAndSetsId()
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

        var newUser = (await repository.ListAsync(CancellationToken.None))
                        .FirstOrDefault();

        newUser.ShouldNotBeNull();
        user.Name.ShouldBe(newUser.Name);
        user.Email.ShouldBe(newUser.Email);
        newUser.Id.ShouldBeGreaterThan(0);
    }
}
