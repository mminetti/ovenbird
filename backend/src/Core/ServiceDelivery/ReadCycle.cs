namespace Core.ServiceDelivery;

public class ReadCycle : EntityBase<int>, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
}
