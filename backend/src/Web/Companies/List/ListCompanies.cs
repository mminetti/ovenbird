using Ardalis.Result;
using UseCases.Common;
using UseCases.Companies;
using UseCases.Companies.List;
using Web.Resources;

namespace Web.Companies.List;

public class ListCompanies(IMessageBus bus) : Endpoint<ListCompaniesRequest, ListCompaniesResponse, ListCompaniesMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListCompaniesRequest.Route);
        Permissions(Constants.Permissions.CompaniesRead);

        Summary(s =>
        {
            s.Summary = "List companies";
            s.Description = "Retrieves a paginated list of all companies.";
            s.ExampleRequest = new ListCompaniesRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, Constants.Pagination.MaxPageSize, Constants.Pagination.DefaultPageSize);
            s.Params["search"] = string.Format(Endpoints.ParamSearch, "name");
            s.Params["order_by"] = Endpoints.ParamOrderBy;

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Companies");

        Description(builder => builder
            .Accepts<ListCompaniesRequest>()
            .Produces<ListCompaniesResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListCompaniesRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemPagedResult<CompanyDto>>>(
            new ListCompaniesQuery(request.Page, request.PerPage, request.Search, request.OrderBy), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListCompaniesMapper
    : Mapper<ListCompaniesRequest, ListCompaniesResponse, ItemPagedResult<CompanyDto>>
{
    public override ListCompaniesResponse FromEntity(ItemPagedResult<CompanyDto> e)
    {
        var items = e.Items
            .Select(c => new CompanyRecord(
                c.Id,
                c.Name,
                c.MarketId,
                c.MarketName,
                c.TimeZoneId))
            .ToList();

        return new ListCompaniesResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
