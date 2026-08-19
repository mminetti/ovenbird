using FluentValidation;

namespace Web.Security.Users.List;

public sealed class ListUsersRequest
{
    public const string Route = "/security/users";

    [BindFrom("page")]
    public int Page { get; init; } = 1;

    [BindFrom("per_page")]
    public int PerPage { get; init; } = UseCases.Constants.DEFAULT_PAGE_SIZE;
}

public sealed class ListUsersValidator : Validator<ListUsersRequest>
{
    public ListUsersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("page must be >= 1");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, UseCases.Constants.MAX_PAGE_SIZE)
            .WithMessage($"per_page must be between 1 and {UseCases.Constants.MAX_PAGE_SIZE}");
    }
}
