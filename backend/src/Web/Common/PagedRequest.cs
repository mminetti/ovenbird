using FluentValidation;
using UseCases.Common;

namespace Web.Common;

public abstract class PagedRequest
{
    [BindFrom("page")]
    public int Page { get; init; } = 1;

    [BindFrom("per_page")]
    public int PerPage { get; init; } = Constants.DEFAULT_PAGE_SIZE;
}

public abstract class PagedRequestValidator<T> : Validator<T> where T : PagedRequest
{
    protected PagedRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("page must be >= 1");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, Constants.MAX_PAGE_SIZE)
            .WithMessage($"per_page must be between 1 and {Constants.MAX_PAGE_SIZE}");
    }
}
