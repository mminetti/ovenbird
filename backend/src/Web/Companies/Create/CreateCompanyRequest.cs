using FluentValidation;

namespace Web.Companies.Create;

public class CreateCompanyRequest
{
    public const string Route = "/companies";

    public string Name { get; set; } = string.Empty;
    public int MarketId { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
}

public class CreateCompanyValidator : Validator<CreateCompanyRequest>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(250);

        RuleFor(x => x.MarketId)
            .GreaterThan(0).WithMessage("Market is required.");

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone is required.")
            .MaximumLength(250)
            .Must(BeAValidTimeZone).WithMessage("Time zone is not a recognized time zone identifier.");
    }

    private static bool BeAValidTimeZone(string timeZoneId) =>
        !string.IsNullOrWhiteSpace(timeZoneId) && TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _);
}
