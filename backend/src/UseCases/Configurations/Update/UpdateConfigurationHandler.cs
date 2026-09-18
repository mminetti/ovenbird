using Core.Shared;
using Core.Shared.Specifications;

namespace UseCases.Configurations.Update;

public class UpdateConfigurationHandler(
    IRepository<Configuration> repository,
    IReadRepository<Connector> connectorRepository)
{
    public async Task<Result> Handle(UpdateConfigurationCommand command, CancellationToken ct)
    {
        var configuration = await repository.FirstOrDefaultAsync(new ConfigurationByIdSpec(command.ConfigurationId), ct);

        if (configuration is null)
        {
            return Result.NotFound();
        }

        configuration.Name = command.Name;
        configuration.Description = command.Description;
        configuration.ConfigurationTypeId = command.ConfigurationTypeId;
        configuration.CompanyId = command.CompanyId;

        var connectors = command.ConnectorIds.Count > 0
            ? await connectorRepository.ListAsync(new ConnectorsByIdsSpec(command.ConnectorIds), ct)
            : [];

        configuration.Connectors.Clear();

        foreach (var connector in connectors)
        {
            configuration.Connectors.Add(connector);
        }

        configuration.ConfigurationFields.Clear();

        foreach (var field in command.Fields)
        {
            configuration.ConfigurationFields.Add(new ConfigurationField
            {
                Name = field.Name,
                Value = field.Value
            });
        }

        await repository.UpdateAsync(configuration, ct);

        return Result.Success();
    }
}
