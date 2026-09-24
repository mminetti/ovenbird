using Ardalis.Result;
using UseCases.Auditing;
using UseCases.Auditing.List;
using UseCases.Common;
using Web.Resources;

namespace Web.Auditing.List;

public class ListAuditTrail(IMessageBus bus) : Endpoint<ListAuditTrailRequest, ListAuditTrailResponse, ListAuditTrailMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListAuditTrailRequest.Route);
        Permissions(Constants.Permissions.AuditTrailRead);

        Summary(s =>
        {
            s.Summary = "List audit trail entries";
            s.Description = "Retrieves a paginated history of changes for a single entity, including changes to its child entities and join-table memberships.";
            s.ExampleRequest = new ListAuditTrailRequest { EntityType = "Configuration", EntityId = "1", Page = 1, PerPage = 10 };

            s.Params["entity_type"] = "The audited entity type to look up, e.g. \"Configuration\".";
            s.Params["entity_id"] = "The entity's id, as a string.";
            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, Constants.Pagination.MaxPageSize, Constants.Pagination.DefaultPageSize);

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Auditing");

        Description(builder => builder
            .Accepts<ListAuditTrailRequest>()
            .Produces<ListAuditTrailResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListAuditTrailRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemPagedResult<AuditTrailDto>>>(
            new ListAuditTrailQuery(request.EntityType, request.EntityId, request.Page, request.PerPage), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListAuditTrailMapper
    : Mapper<ListAuditTrailRequest, ListAuditTrailResponse, ItemPagedResult<AuditTrailDto>>
{
    public override ListAuditTrailResponse FromEntity(ItemPagedResult<AuditTrailDto> e)
    {
        var items = e.Items
            .Select(a => new AuditTrailRecord(
                a.Id,
                a.EntityType,
                a.EntityId,
                a.Action.ToString(),
                a.UserId,
                a.TimestampUtc,
                a.OldValues,
                a.NewValues,
                a.AffectedColumns))
            .ToList();

        return new ListAuditTrailResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
