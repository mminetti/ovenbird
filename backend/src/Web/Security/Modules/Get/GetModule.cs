using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Modules;
using UseCases.Security.Modules.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Modules.Get;

public class GetModule(IMessageBus bus)
    : Endpoint<GetModuleRequest,
               Results<Ok<ModuleRecord>, NotFound, ProblemHttpResult>,
               GetModuleByIdMapper>
{
    public override void Configure()
    {
        Get(GetModuleRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Get a module";
            s.Description = "Retrieves a specific module by its unique identifier.";
            s.ExampleRequest = new GetModuleRequest { ModuleId = 1 };

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<GetModuleRequest>()
            .Produces<ModuleRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<ModuleRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetModuleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<ModuleDto>>(new GetModuleQuery(request.ModuleId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetModuleByIdMapper : Mapper<GetModuleRequest, ModuleRecord, ModuleDto>
{
    public override ModuleRecord FromEntity(ModuleDto e) => new(e.Id, e.Name);
}
