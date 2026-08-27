namespace Core.Shared;

public class ConfigurationType : EntityBase<int>
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Configuration> Configurations { get; set; } = [];
}
