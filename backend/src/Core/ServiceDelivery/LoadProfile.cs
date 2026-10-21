namespace Core.ServiceDelivery;

public class LoadProfile : EntityBase<int>, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
}
