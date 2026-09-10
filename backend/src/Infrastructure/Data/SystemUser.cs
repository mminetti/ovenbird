using UseCases.Common;

namespace Infrastructure.Data;

/// <summary>
/// Default <see cref="IUser"/> for hosts with no HTTP context (e.g. the Worker service).
/// The Web host overrides this registration with HttpUser.
/// </summary>
public class SystemUser : IUser
{
    public string? Id => "system";
}
