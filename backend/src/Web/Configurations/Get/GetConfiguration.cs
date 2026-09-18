using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Configurations;
using UseCases.Configurations.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Configurations.Get;

public class GetConfiguration(IMessageBus bus)
    : Endpoint<GetConfigurationRequest,
               Results<Ok<ConfigurationRecord>, NotFound, ProblemHttpResult>,
               GetConfigurationMapper>
{
    public override void Configure()
    {
        Get(GetConfigurationRequest.Route);
        Permissions(Constants.Permissions.ConfigurationsRead);

        Summary(s =>
        {
            s.Summary = "Get a configuration";
            s.Description = "Retrieves a specific configuration by its unique identifier.";
            s.ExampleRequest = new GetConfigurationRequest { ConfigurationId = 1 };

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Configurations");

        Description(builder => builder
            .Accepts<GetConfigurationRequest>()
            .Produces<ConfigurationRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<ConfigurationRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetConfigurationRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<ConfigurationDto>>(new GetConfigurationQuery(request.ConfigurationId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetConfigurationMapper : Mapper<GetConfigurationRequest, ConfigurationRecord, ConfigurationDto>
{
    public override ConfigurationRecord FromEntity(ConfigurationDto e)
    {
        var fields = e.Fields
            .Select(f => new ConfigurationFieldRecord(f.Id, f.Name, f.Value))
            .ToList();

        var connectors = e.Connectors
            .Select(c => new ConfigurationConnectorRecord(c.Id, c.Name))
            .ToList();

        return new ConfigurationRecord(
            e.Id,
            e.Name,
            e.Description,
            e.ConfigurationTypeId,
            e.ConfigurationTypeName,
            e.CompanyId,
            e.CompanyName)
        {
            Fields = fields,
            Connectors = connectors
        };
    }
}
