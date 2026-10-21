namespace Core.ServiceDelivery;

public class CustomerClass : EntityBase<int>, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
}
