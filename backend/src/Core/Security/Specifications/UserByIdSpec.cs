namespace Core.Security.Specifications;

public class UserByIdSpec : Specification<User>
{
    public UserByIdSpec(int userId) =>
        Query.Where(user => user.Id == userId);
}
