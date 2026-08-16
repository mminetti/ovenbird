using Core.ContributorAggregate;

namespace IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
    [Fact]
    public async Task AddsContributorAndSetsId()
    {
        var testContributorName = "testContributor";
        var testContributorStatus = ContributorStatus.NotSet;
        var repository = GetRepository();
        var Contributor = new Contributor(testContributorName);

        await repository.AddAsync(Contributor, CancellationToken.None);

        var newContributor = (await repository.ListAsync(CancellationToken.None))
                        .FirstOrDefault();

        newContributor.ShouldNotBeNull();
        testContributorName.ShouldBe(newContributor.Name);
        testContributorStatus.ShouldBe(newContributor.Status);
        newContributor.Id.ShouldBeGreaterThan(0);
    }
}
