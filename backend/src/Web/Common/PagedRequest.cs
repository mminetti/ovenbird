using FluentValidation;
using UseCases.Common;

namespace Web.Common;

public abstract class PagedRequest
{
    [BindFrom("page")]
    public int Page { get; init; } = 1;

    [BindFrom("per_page")]
    public int PerPage { get; init; } = Constants.Pagination.DefaultPageSize;
    [BindFrom("search")]
    public string? Search { get; init; }

    [BindFrom("order_by")]
    public string? OrderBy { get; init; }
}

public abstract class PagedRequestValidator<T> : Validator<T> where T : PagedRequest
{
    protected PagedRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("page must be >= 1");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, Constants.Pagination.MaxPageSize)
            .WithMessage($"per_page must be between 1 and {Constants.Pagination.MaxPageSize}");

        RuleFor(x => x.Search)
            .MaximumLength(Constants.Pagination.MaxSearchLength)
            .WithMessage($"search must be at most {Constants.Pagination.MaxSearchLength} characters");
    }
}
