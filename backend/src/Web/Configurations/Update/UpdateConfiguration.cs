using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Configurations;
using UseCases.Configurations.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Configurations.Update;

public class UpdateConfiguration(IMessageBus bus)
    : Endpoint<UpdateConfigurationRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdateConfigurationRequest.Route);
        Permissions(Constants.Permissions.ConfigurationsWrite);

        Summary(s =>
        {
            s.Summary = "Update a configuration";
            s.Description = "Updates an existing configuration's details, type, company, connectors, and fields.";
            s.ExampleRequest = new UpdateConfigurationRequest
            {
                ConfigurationId = 1,
                Id = 1,
                Name = "Acme EDI Import",
                ConfigurationTypeId = 1,
                CompanyId = 1,
                ConnectorIds = [1, 2],
                Fields = [new ConfigurationFieldRequest { Name = "batchSize", Value = "100" }]
            };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Configurations");

        Description(builder => builder
            .Accepts<UpdateConfigurationRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateConfigurationRequest request, CancellationToken ct)
    {
        var fields = request.Fields
            .Select(f => new ConfigurationFieldInput(f.Name, f.Value))
            .ToList();

        var result = await bus.InvokeAsync<Result>(
            new UpdateConfigurationCommand(
                request.ConfigurationId,
                request.Name,
                request.Description,
                request.ConfigurationTypeId,
                request.CompanyId,
                request.ConnectorIds,
                fields), ct);

        return result.ToDeleteUpdateResult();
    }
}
