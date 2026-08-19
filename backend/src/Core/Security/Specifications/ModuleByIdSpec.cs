namespace Core.Security.Specifications;

public class ModuleByIdSpec : Specification<Module>
{
    public ModuleByIdSpec(int moduleId) =>
        Query.Where(module => module.Id == moduleId);
}
