using Core.Settings;
using Core.Settings.Specifications;

namespace UseCases.Connectors.Update;

public class UpdateConnectorHandler(IRepository<Connector> repository)
{
    public async Task<Result> Handle(UpdateConnectorCommand command, CancellationToken ct)
    {
        var connector = await repository.FirstOrDefaultAsync(new ConnectorByIdSpec(command.ConnectorId), ct);

        if (connector is null)
        {
            return Result.NotFound();
        }

        connector.Name = command.Name;
        connector.Description = command.Description;
        connector.ConnectorImplementationId = command.ConnectorImplementationId;

        foreach (var field in command.Fields)
        {
            switch (field.Operation)
            {
                case "create":
                    connector.ConnectorFields.Add(new ConnectorField
                    {
                        Name = field.Name,
                        Value = field.Value,
                        IsSecret = field.IsSecret
                    });
                    break;

                case "update":
                    var existingField = connector.ConnectorFields.FirstOrDefault(f => f.Id == field.Id);
                    if (existingField is not null)
                    {
                        existingField.Name = field.Name;
                        existingField.Value = field.Value;
                        existingField.IsSecret = field.IsSecret;
                    }
                    break;

                case "delete":
                    var fieldToRemove = connector.ConnectorFields.FirstOrDefault(f => f.Id == field.Id);
                    if (fieldToRemove is not null)
                    {
                        connector.ConnectorFields.Remove(fieldToRemove);
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Invalid field operation '{field.Operation}'.");
            }
        }

        await repository.UpdateAsync(connector, ct);

        return Result.Success();
    }
}
