using FluentValidation;

namespace Web.Connectors.Update;

public class UpdateConnectorRequest
{
    public const string Route = "/connectors/{ConnectorId:int}";
    public static string BuildRoute(int connectorId) => Route.Replace("{ConnectorId:int}", connectorId.ToString());

    public int ConnectorId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConnectorTypeId { get; set; }
    public int ConnectorImplementationId { get; set; }
    public IReadOnlyList<ConnectorFieldRequest> Fields { get; set; } = [];
}

public class UpdateConnectorValidator : Validator<UpdateConnectorRequest>
{
    public UpdateConnectorValidator()
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

        RuleFor(x => x.ConnectorId)
            .Must((args, connectorId) => args.Id == connectorId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }
}
