using Core.Shared;

namespace UseCases.Connectors.Create;

public class CreateConnectorHandler(IRepository<Connector> repository)
{
    public async Task<Result<int>> Handle(CreateConnectorCommand command, CancellationToken ct)
    {
        var connector = new Connector
        {
            Name = command.Name,
            Description = command.Description,
            ConnectorTypeId = command.ConnectorTypeId,
            ConnectorImplementationId = command.ConnectorImplementationId,
            ConnectorFields = [.. command.Fields
                .Select(f => new ConnectorField
                {
                    Name = f.Name,
                    Value = f.Value,
                    IsSecret = f.IsSecret
                })]
        };

        var created = await repository.AddAsync(connector, ct);

        return created.Id;
    }
}
