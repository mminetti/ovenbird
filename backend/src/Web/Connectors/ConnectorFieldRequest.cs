using FluentValidation;

namespace Web.Connectors;

public class ConnectorFieldRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public bool IsSecret { get; set; }
}

public class ConnectorFieldRequestValidator : Validator<ConnectorFieldRequest>
{
    public ConnectorFieldRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Field name is required.")
            .MaximumLength(200);
    }
}
