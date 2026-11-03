namespace UseCases.Interfaces.ServiceDelivery;

public interface IUpdateServicePointService
{
    Task<Result> ExecuteAsync(UpdateServicePointRequest request, CancellationToken ct);
}
