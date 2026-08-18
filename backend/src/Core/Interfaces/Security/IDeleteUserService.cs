namespace Core.Interfaces.Security;

public interface IDeleteUserService
{
    // This service and method exist to provide a place in which to fire domain events
    // when deleting this entity
    public Task<Result> DeleteUserAsync(int userId, CancellationToken ct);
}
