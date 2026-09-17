using Core.Shared;
using Core.Shared.Specifications;

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
        connector.ConnectorTypeId = command.ConnectorTypeId;
        connector.ConnectorImplementationId = command.ConnectorImplementationId;

        connector.ConnectorFields.Clear();

        foreach (var field in command.Fields)
        {
            connector.ConnectorFields.Add(new ConnectorField
            {
                Name = field.Name,
                Value = field.Value,
                IsSecret = field.IsSecret
            });
        }

        await repository.UpdateAsync(connector, ct);

        return Result.Success();
    }
}
