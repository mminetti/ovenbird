using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.DataLists;
using UseCases.DataLists.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.DataLists.Get;

/// <summary>
/// Endpoint to retrieve reference data for UI dropdowns.
/// </summary>
public class GetDataList(IMessageBus bus) : Endpoint<GetDataListRequest, Ok<GetDataListResponse>, GetDataListMapper>
{
    public override void Configure()
    {
        Get(GetDataListRequest.Route);

        Summary(s =>
        {
            s.Summary = "Get a data list";
            s.Description = "Retrieves reference data for populating UI dropdowns. Returns Id-Name pairs for the specified type.";

            s.Params["Type"] = "Type of data list: " + string.Join(", ", Enum.GetNames<DataListType>());

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Data-Lists");

        Description(builder => builder
            .Accepts<GetDataListRequest>()
            .Produces<GetDataListResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Ok<GetDataListResponse>> ExecuteAsync(GetDataListRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<DataListResponse>>(new GetDataListQuery(request.Type), ct);

        return result.ToOkOnlyResult(Map.FromEntity);
    }
}

public sealed class GetDataListMapper : Mapper<GetDataListRequest, GetDataListResponse, DataListResponse>
{
    public override GetDataListResponse FromEntity(DataListResponse e)
    {
        var items = e.Items
            .Select(x => new ValuePairRecord(x.Id, x.Name))
            .ToList();

        return new GetDataListResponse(e.Type, items);
    }
}
