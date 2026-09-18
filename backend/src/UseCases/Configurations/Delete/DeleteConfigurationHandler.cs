using Core.Shared;

namespace UseCases.Configurations.Delete;

public class DeleteConfigurationHandler(IRepository<Configuration> repository)
{
    public async Task<Result> Handle(DeleteConfigurationCommand command, CancellationToken ct)
    {
        var configuration = await repository.GetByIdAsync(command.ConfigurationId, ct);

        if (configuration is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(configuration, ct);

        return Result.Success();
    }
}
