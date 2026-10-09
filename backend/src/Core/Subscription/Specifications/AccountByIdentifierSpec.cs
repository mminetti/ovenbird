namespace Core.Subscription.Specifications;

public class AccountByIdentifierSpec : Specification<Account>
{
    public AccountByIdentifierSpec(string identifier) =>
        Query.Where(account => account.Identifier == identifier);
}
