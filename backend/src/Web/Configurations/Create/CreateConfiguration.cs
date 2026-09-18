using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Configurations;
using UseCases.Configurations.Create;
using Web.Configurations.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Configurations.Create;

public class CreateConfiguration(IMessageBus bus)
    : Endpoint<CreateConfigurationRequest,
               Results<Created<CreateConfigurationResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateConfigurationRequest.Route);
        Permissions(Constants.Permissions.ConfigurationsWrite);

        Summary(s =>
        {
            s.Summary = "Create a configuration";
            s.Description = "Creates a new configuration with the provided type, company, connectors, and fields.";
            s.ExampleRequest = new CreateConfigurationRequest
            {
                Name = "Acme EDI Import",
                ConfigurationTypeId = 1,
                CompanyId = 1,
                ConnectorIds = [1, 2],
                Fields = [new ConfigurationFieldRequest { Name = "batchSize", Value = "100" }]
            };

            s.Responses[201] = Endpoints.Response201Created;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Configurations");

        Description(builder => builder
            .Accepts<CreateConfigurationRequest>("application/json")
            .Produces<CreateConfigurationResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreateConfigurationResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateConfigurationRequest request, CancellationToken ct)
    {
        var fields = request.Fields
            .Select(f => new ConfigurationFieldInput(f.Name, f.Value))
            .ToList();

        var result = await bus.InvokeAsync<Result<int>>(
            new CreateConfigurationCommand(
                request.Name,
                request.Description,
                request.ConfigurationTypeId,
                request.CompanyId,
                request.ConnectorIds,
                fields), ct);

        return result.ToCreatedResult(
            id => GetConfigurationRequest.BuildRoute(id),
            id => new CreateConfigurationResponse(id));
    }
}
