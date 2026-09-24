namespace Core.Security;

public class PermissionModule : EntityBase<int>
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = [];
}
