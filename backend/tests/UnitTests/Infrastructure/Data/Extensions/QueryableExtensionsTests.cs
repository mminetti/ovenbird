using Infrastructure.Data.Extensions;

namespace UnitTests.Infrastructure.Data.Extensions;

public class QueryableExtensionsTests
{
    private record Address(string City);
    private record Person(int Id, string Name, Address Home);

    private static readonly IQueryable<Person> _people = new List<Person>
    {
        new(1, "Zoe Adams", new Address("Boston")),
        new(2, "Mike Brooks", new Address("Austin")),
        new(3, "Alice Carter", new Address("Austin")),
    }.AsQueryable();

    [Fact]
    public void OrderByDynamic_SortsByNestedPropertyPath()
    {
        var result = _people.OrderByDynamic("Home.City").Select(x => x.Id).ToList();

        result.ShouldBe([2, 3, 1]);
    }

    [Fact]
    public void OrderByDynamic_SortsByNestedPropertyPathDescending()
    {
        var result = _people.OrderByDynamic("Home.City desc").Select(x => x.Id).ToList();

        result.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void OrderByDynamic_FallsBackToNoOpWhenNestedSegmentMissing()
    {
        var result = _people.OrderByDynamic("Home.Country").Select(x => x.Id).ToList();

        result.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void WhereContainsText_MatchesOnNestedPropertyPath()
    {
        var result = _people.WhereContainsText("Austin", "Home.City").Select(x => x.Id).ToList();

        result.ShouldBe([2, 3]);
    }
}
