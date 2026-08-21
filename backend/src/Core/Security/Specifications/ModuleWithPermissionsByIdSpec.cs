namespace Core.Security.Specifications;

public class ModuleWithPermissionsByIdSpec : Specification<Module>
{
    public ModuleWithPermissionsByIdSpec(int moduleId) =>
        Query
            .Where(module => module.Id == moduleId)
            .Include(module => module.Permissions);
}
