namespace Core.ServiceDelivery;

public class County : EntityBase<int>, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
}
