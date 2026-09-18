using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Companies.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Companies.Update;

public class UpdateCompany(IMessageBus bus)
    : Endpoint<UpdateCompanyRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdateCompanyRequest.Route);
        Permissions(Constants.Permissions.CompaniesWrite);

        Summary(s =>
        {
            s.Summary = "Update a company";
            s.Description = "Updates an existing company's details.";
            s.ExampleRequest = new UpdateCompanyRequest
            {
                CompanyId = 1,
                Id = 1,
                Name = "Acme Corp",
                MarketId = 1,
                TimeZoneId = "America/New_York"
            };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Companies");

        Description(builder => builder
            .Accepts<UpdateCompanyRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateCompanyRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(
            new UpdateCompanyCommand(request.CompanyId, request.Name, request.MarketId, request.TimeZoneId), ct);

        return result.ToDeleteUpdateResult();
    }
}
