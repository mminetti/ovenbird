using Core.Shared;
using Core.Shared.Specifications;

namespace UseCases.Configurations.Create;

public class CreateConfigurationHandler(
    IRepository<Configuration> repository,
    IRepository<Connector> connectorRepository)
{
    public async Task<Result<int>> Handle(CreateConfigurationCommand command, CancellationToken ct)
    {
        var connectors = command.ConnectorIds.Count > 0
            ? await connectorRepository.ListAsync(new ConnectorsByIdsSpec(command.ConnectorIds), ct)
            : [];

        var configuration = new Configuration
        {
            Name = command.Name,
            Description = command.Description,
            ConfigurationTypeId = command.ConfigurationTypeId,
            CompanyId = command.CompanyId,
            Connectors = connectors,
            ConfigurationFields = [.. command.Fields
                .Select(f => new ConfigurationField
                {
                    Name = f.Name,
                    Value = f.Value
                })]
        };

        var created = await repository.AddAsync(configuration, ct);

        return created.Id;
    }
}
