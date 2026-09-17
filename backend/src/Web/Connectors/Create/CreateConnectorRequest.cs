using FluentValidation;

namespace Web.Connectors.Create;

public class CreateConnectorRequest
{
    public const string Route = "/connectors";

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConnectorTypeId { get; set; }
    public int ConnectorImplementationId { get; set; }
    public IReadOnlyList<ConnectorFieldRequest> Fields { get; set; } = [];
}

public class CreateConnectorValidator : Validator<CreateConnectorRequest>
{
    public CreateConnectorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ConnectorTypeId)
            .GreaterThan(0).WithMessage("Connector type is required.");

        RuleFor(x => x.ConnectorImplementationId)
            .GreaterThan(0).WithMessage("Connector implementation is required.");

        RuleForEach(x => x.Fields).SetValidator(new ConnectorFieldRequestValidator());
    }
}
