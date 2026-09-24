using Core.Shared;
using Core.Shared.Specifications;

namespace UseCases.Configurations.Get;

public class GetConfigurationHandler(IReadRepository<Configuration> repository)
{
    public async Task<Result<ConfigurationDto>> Handle(GetConfigurationQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new ConfigurationByIdSpec(request.ConfigurationId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        var fields = entity.ConfigurationFields
            .Select(f => new ConfigurationFieldDto(f.Id, f.Name, f.Value))
            .ToList();

        var connectors = entity.Connectors
            .Select(c => new ConfigurationConnectorDto(c.Id, c.Name))
            .ToList();

        return Result.Success(new ConfigurationDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.ConfigurationTypeId,
            entity.ConfigurationType.Name,
            entity.CompanyId,
            entity.Company?.Name,
            entity.LastModifiedAtUtc,
            entity.LastModifiedBy)
        {
            Fields = fields,
            Connectors = connectors
        });
    }
}
