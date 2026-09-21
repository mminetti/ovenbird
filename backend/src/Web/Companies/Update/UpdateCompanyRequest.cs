using FluentValidation;

namespace Web.Companies.Update;

public class UpdateCompanyRequest
{
    public const string Route = "/settings/companies/{CompanyId:int}";
    public static string BuildRoute(int companyId) => Route.Replace("{CompanyId:int}", companyId.ToString());

    public int CompanyId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MarketId { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
}

public class UpdateCompanyValidator : Validator<UpdateCompanyRequest>
{
    public UpdateCompanyValidator()
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

        RuleFor(x => x.CompanyId)
            .Must((args, companyId) => args.Id == companyId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }

    private static bool BeAValidTimeZone(string timeZoneId) =>
        !string.IsNullOrWhiteSpace(timeZoneId) && TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _);
}
