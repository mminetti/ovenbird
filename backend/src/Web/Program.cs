using Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()    // This sets up OpenTelemetry logging
       .AddLoggerConfigs();     // This adds Serilog for console formatting

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddOptionConfigs(builder.Configuration, startupLogger, builder);
builder.Services.AddServiceConfigs(startupLogger, builder);

var entraEnabled = builder.Configuration.GetValue<bool>("Authentication:AzureAdEnabled");

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                    o.DocumentSettings = s =>
                    {
                        s.Title = "Clean Architecture API";
                        s.Version = "v1";
                        s.Description = "HTTP endpoints for the Clean Architecture sample application.";

                        if (entraEnabled)
                        {
                            var tenantId = builder.Configuration["AzureAd:TenantId"] ?? "common";
                            var clientId = builder.Configuration["AzureAd:ClientId"] ?? string.Empty;
                            var baseUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0";

                            s.AddAuth("oauth2", new NSwag.OpenApiSecurityScheme
                            {
                                Type = NSwag.OpenApiSecuritySchemeType.OAuth2,
                                Flows = new NSwag.OpenApiOAuthFlows
                                {
                                    AuthorizationCode = new NSwag.OpenApiOAuthFlow
                                    {
                                        AuthorizationUrl = $"{baseUrl}/authorize",
                                        TokenUrl = $"{baseUrl}/token",
                                        Scopes = new Dictionary<string, string>
                                    {
                                        {
                                            builder.Configuration["AzureAd:Scopes"] ?? $"{clientId}/.default",
                                            "Access API as user"
                                        }
                                    }
                                    }
                                }
                            });

                            s.OperationProcessors.Add(new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("oauth2"));
                        }
                    };
                    o.ShortSchemaNames = true;
                });

var app = builder.Build();

await app.UseAppMiddlewareAndSeedDatabase();

app.MapDefaultEndpoints(); // Aspire health checks and metrics

app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }
