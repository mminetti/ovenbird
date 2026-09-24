using Core.Shared;
using Core.Shared.Specifications;

namespace UseCases.Connectors.Get;

public class GetConnectorHandler(IReadRepository<Connector> repository)
{
    public async Task<Result<ConnectorDto>> Handle(GetConnectorQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new ConnectorByIdSpec(request.ConnectorId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        var fields = entity.ConnectorFields
            .Select(f => new ConnectorFieldDto(f.Id, f.Name, f.Value, f.IsSecret))
            .ToList();

        return Result.Success(new ConnectorDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.ConnectorImplementation.ConnectorTypeId,
            entity.ConnectorImplementation.ConnectorType.Name,
            entity.ConnectorImplementationId,
            entity.ConnectorImplementation.Name,
            entity.LastModifiedAtUtc,
            entity.LastModifiedBy)
        {
            Fields = fields
        });
    }
}
