namespace UseCases.Interfaces.ServiceDelivery;

public interface IUpdateServicePointAction
{
    Task<Result> ExecuteAsync(UpdateServicePointRequest request, CancellationToken ct);
}
