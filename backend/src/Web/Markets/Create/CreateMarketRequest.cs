using FluentValidation;

namespace Web.Markets.Create;

public class CreateMarketRequest
{
    public const string Route = "/settings/markets";

    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
}

public class CreateMarketValidator : Validator<CreateMarketRequest>
{
    public CreateMarketValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(250);

        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("Identifier is required.")
            .MaximumLength(250);
    }
}
