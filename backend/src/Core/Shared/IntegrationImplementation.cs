namespace Core.Shared;

public class IntegrationImplementation : EntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Integration> Integrations { get; set; } = [];
}
