using Core.Common;

namespace Core.Settings;

public class ConfigurationType : EntityBase<int>, IHasDisplayName
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Configuration> Configurations { get; set; } = [];
}
