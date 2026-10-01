using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common.Constants;
using UseCases.Settings.Companies;
using UseCases.Settings.Companies.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Endpoints.Settings.Companies.Get;

public class GetCompany(IMessageBus bus)
    : Endpoint<GetCompanyRequest,
               Results<Ok<CompanyRecord>, NotFound, ProblemHttpResult>,
               GetCompanyMapper>
{
    public override void Configure()
    {
        Get(GetCompanyRequest.Route);
        Permissions(Constants.Permissions.CompaniesRead);

        Summary(s =>
        {
            s.Summary = "Get a company";
            s.Description = "Retrieves a specific company by its unique identifier.";
            s.ExampleRequest = new GetCompanyRequest { CompanyId = 1 };

            s.Responses[200] = EndpointSummaries.Response200Ok;
            s.Responses[400] = EndpointSummaries.Response400BadRequest;
            s.Responses[404] = EndpointSummaries.Response404NotFound;
            s.Responses[500] = EndpointSummaries.Response500InternalServerError;
        });

        Tags("Companies");

        Description(builder => builder
            .Accepts<GetCompanyRequest>()
            .Produces<CompanyRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<CompanyRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetCompanyRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<CompanyDto>>(new GetCompanyQuery(request.CompanyId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetCompanyMapper : Mapper<GetCompanyRequest, CompanyRecord, CompanyDto>
{
    public override CompanyRecord FromEntity(CompanyDto e) =>
        new(e.Id, e.Name, e.MarketId, e.MarketName, e.TimeZoneId, e.LastModifiedAtUtc, e.LastModifiedBy);
}
