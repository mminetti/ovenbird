using FluentValidation;

namespace Web.Configurations.Create;

public class CreateConfigurationRequest
{
    public const string Route = "/configurations";

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConfigurationTypeId { get; set; }
    public int? CompanyId { get; set; }
    public IReadOnlyList<int> ConnectorIds { get; set; } = [];
    public IReadOnlyList<ConfigurationFieldRequest> Fields { get; set; } = [];
}

public class CreateConfigurationValidator : Validator<CreateConfigurationRequest>
{
    public CreateConfigurationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ConfigurationTypeId)
            .GreaterThan(0).WithMessage("Configuration type is required.");

        RuleForEach(x => x.Fields).SetValidator(new ConfigurationFieldRequestValidator());
    }
}
