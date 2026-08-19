namespace Infrastructure.Data;

// ReadDbContext inherits all DbSets and configuration from AppDbContext
public class ReadDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
{

}
