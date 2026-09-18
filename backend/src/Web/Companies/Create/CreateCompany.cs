using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Companies.Create;
using Web.Companies.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Companies.Create;

public class CreateCompany(IMessageBus bus)
    : Endpoint<CreateCompanyRequest,
               Results<Created<CreateCompanyResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateCompanyRequest.Route);
        Permissions(Constants.Permissions.CompaniesWrite);

        Summary(s =>
        {
            s.Summary = "Create a company";
            s.Description = "Creates a new company for the given market.";
            s.ExampleRequest = new CreateCompanyRequest
            {
                Name = "Acme Corp",
                MarketId = 1,
                TimeZoneId = "America/New_York"
            };

            s.Responses[201] = Endpoints.Response201Created;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Companies");

        Description(builder => builder
            .Accepts<CreateCompanyRequest>("application/json")
            .Produces<CreateCompanyResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreateCompanyResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateCompanyRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<int>>(
            new CreateCompanyCommand(request.Name, request.MarketId, request.TimeZoneId), ct);

        return result.ToCreatedResult(
            id => GetCompanyRequest.BuildRoute(id),
            id => new CreateCompanyResponse(id));
    }
}
