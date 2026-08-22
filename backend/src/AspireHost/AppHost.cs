
var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sqlServer = builder.AddSqlServer("sqlserver")
  .WithLifetime(ContainerLifetime.Persistent);

// Add the database
var db = sqlServer.AddDatabase("db");

// Add the web project with the database connection
builder.AddProject<Projects.Web>("api")
    .WithReference(db)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WaitFor(db);

builder.AddProject<Projects.Worker>("worker")
    .WithReference(db)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WaitFor(db);

builder
  .Build()
  .Run();
