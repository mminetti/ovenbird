namespace Core.Security.Specifications;

public class UserByExternalIdentifierSpec : Specification<User>
{
    public UserByExternalIdentifierSpec(string externalIdentifier) =>
        Query.Where(user => user.ExternalIdentifier == externalIdentifier);
}
