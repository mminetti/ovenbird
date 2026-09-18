using FluentValidation;

namespace Web.Configurations;

public class ConfigurationFieldRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
}

public class ConfigurationFieldRequestValidator : Validator<ConfigurationFieldRequest>
{
    public ConfigurationFieldRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Field name is required.")
            .MaximumLength(200);
    }
}
