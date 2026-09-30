using Core.Shared;
using Core.Shared.Specifications;

namespace UseCases.Configurations.Update;

public class UpdateConfigurationHandler(
    IRepository<Configuration> repository,
    IRepository<Connector> connectorRepository)
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

        foreach (var field in command.Fields)
        {
            switch (field.Operation)
            {
                case "create":
                    configuration.ConfigurationFields.Add(new ConfigurationField
                    {
                        Name = field.Name,
                        Value = field.Value
                    });
                    break;

                case "update":
                    var existingField = configuration.ConfigurationFields.FirstOrDefault(f => f.Id == field.Id);
                    if (existingField is not null)
                    {
                        existingField.Name = field.Name;
                        existingField.Value = field.Value;
                    }
                    break;

                case "delete":
                    var fieldToRemove = configuration.ConfigurationFields.FirstOrDefault(f => f.Id == field.Id);
                    if (fieldToRemove is not null)
                    {
                        configuration.ConfigurationFields.Remove(fieldToRemove);
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Invalid field operation '{field.Operation}'.");
            }
        }

        await repository.UpdateAsync(configuration, ct);

        return Result.Success();
    }
}
