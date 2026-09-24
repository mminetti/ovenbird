using Core.Security;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using UseCases.Common;

namespace IntegrationTests.Data;

public abstract class BaseEfRepoTestFixture
{
    protected AppDbContext _dbContext;
    private readonly DbContextOptions<AppDbContext> _options;

    protected BaseEfRepoTestFixture()
    {
        _options = CreateNewContextOptions();
        _dbContext = new AppDbContext(_options);
    }

    protected static DbContextOptions<AppDbContext> CreateNewContextOptions()
    {
        var fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();
        var fakeUser = Substitute.For<IUser>();
        fakeUser.Id.Returns("test-user");

        // Create a fresh service provider, and therefore a fresh
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .AddScoped<IDomainEventDispatcher>(_ => fakeEventDispatcher)
            .AddScoped<IUser>(_ => fakeUser)
            .AddSingleton(TimeProvider.System)
            .AddScoped<EventDispatchInterceptor>()
            .AddScoped<AuditTrailInterceptor>()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        var eventDispatchInterceptor = serviceProvider.GetRequiredService<EventDispatchInterceptor>();
        var auditTrailInterceptor = serviceProvider.GetRequiredService<AuditTrailInterceptor>();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseInMemoryDatabase("cleanarchitecture")
               .UseInternalServiceProvider(serviceProvider)
               .AddInterceptors(eventDispatchInterceptor, auditTrailInterceptor);

        return builder.Options;
    }

    protected EfRepository<User> GetRepository()
    {
        return new EfRepository<User>(_dbContext);
    }

    protected ReadDbContext GetReadDbContext()
    {
        return new ReadDbContext(_options);
    }
}
