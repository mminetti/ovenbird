using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Companies.Delete;
using Web.Extensions;
using Web.Resources;

namespace Web.Companies.Delete;

public class DeleteCompany(IMessageBus bus)
    : Endpoint<DeleteCompanyRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteCompanyRequest.Route);
        Permissions(Constants.Permissions.CompaniesWrite);

        Summary(s =>
        {
            s.Summary = "Delete a company";
            s.Description = "Deletes an existing company by its unique identifier.";
            s.ExampleRequest = new DeleteCompanyRequest { CompanyId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Companies");

        Description(builder => builder
            .Accepts<DeleteCompanyRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteCompanyRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteCompanyCommand(request.CompanyId), ct);

        return result.ToDeleteUpdateResult();
    }
}
