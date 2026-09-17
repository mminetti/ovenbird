using Core.Shared;

namespace UseCases.Connectors.Delete;

public class DeleteConnectorHandler(IRepository<Connector> repository)
{
    public async Task<Result> Handle(DeleteConnectorCommand command, CancellationToken ct)
    {
        var connector = await repository.GetByIdAsync(command.ConnectorId, ct);

        if (connector is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(connector, ct);

        return Result.Success();
    }
}
