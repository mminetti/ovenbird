namespace UseCases.Interfaces.ServiceDelivery;

public interface IUpdateServicePointFromServiceEventAction
{
    Task<Result> ExecuteAsync(long serviceEventId, CancellationToken ct);
}
