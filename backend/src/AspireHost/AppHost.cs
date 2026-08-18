using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sqlServer = builder.AddSqlServer("sqlserver")
  .WithLifetime(ContainerLifetime.Persistent);

// Add the database
var cleanArchDb = sqlServer.AddDatabase("db");

// Add the web project with the database connection
builder.AddProject<Projects.Web>("api")
  .WithReference(cleanArchDb)
  .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
  .WaitFor(cleanArchDb);

builder
  .Build()
  .Run();
