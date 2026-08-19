namespace Infrastructure.Data;

public class EfReadRepository<T>(ReadDbContext dbContext)
    : RepositoryBase<T>(dbContext), IReadRepository<T> where T : class, IAggregateRoot
{
    // Only IReadRepository
}
