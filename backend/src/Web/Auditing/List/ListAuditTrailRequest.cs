using FluentValidation;
using UseCases.Common;

namespace Web.Auditing.List;

public sealed class ListAuditTrailRequest
{
    public const string Route = "/audit-trail";

    [BindFrom("entity_type")]
    public string EntityType { get; init; } = string.Empty;

    [BindFrom("entity_id")]
    public string EntityId { get; init; } = string.Empty;

    [BindFrom("page")]
    public int Page { get; init; } = 1;

    [BindFrom("per_page")]
    public int PerPage { get; init; } = Constants.Pagination.DefaultPageSize;
}

public sealed class ListAuditTrailValidator : Validator<ListAuditTrailRequest>
{
    public ListAuditTrailValidator()
    {
        RuleFor(x => x.EntityType)
            .NotEmpty()
            .WithMessage("entity_type is required");

        RuleFor(x => x.EntityId)
            .NotEmpty()
            .WithMessage("entity_id is required");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("page must be >= 1");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, Constants.Pagination.MaxPageSize)
            .WithMessage($"per_page must be between 1 and {Constants.Pagination.MaxPageSize}");
    }
}
