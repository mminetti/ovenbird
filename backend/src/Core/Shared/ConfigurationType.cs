namespace Core.Shared;

public class ConfigurationType : EntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Configuration> Configurations { get; set; } = [];
}
