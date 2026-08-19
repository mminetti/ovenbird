using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Modules.Create;
using Web.Extensions;
using Web.Resources;
using Web.Security.Modules.Get;

namespace Web.Security.Modules.Create;

public class CreateModule(IMessageBus bus)
    : Endpoint<CreateModuleRequest,
               Results<Created<CreateModuleResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateModuleRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Create a module";
            s.Description = "Creates a new module with the provided details.";
            s.ExampleRequest = new CreateModuleRequest { Name = "Administration" };

            s.Responses[201] = Endpoints.Response201Created;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<CreateModuleRequest>("application/json")
            .Produces<CreateModuleResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreateModuleResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateModuleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<int>>(new CreateModuleCommand(request.Name), ct);

        return result.ToCreatedResult(
            id => GetModuleRequest.BuildRoute(id),
            id => new CreateModuleResponse(id));
    }
}
