using Infrastructure;
using Infrastructure.Data;
using Quartz;
using Wolverine;
using Worker.Jobs.Market;

var builder = Host.CreateApplicationBuilder(args);

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

builder.Services.AddInfrastructureServices(builder.Configuration, startupLogger);

builder.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Core.Market.MarketDocument).Assembly);
    opts.Discovery.IncludeAssembly(typeof(UseCases.Market.MarketDocuments.Import.ImportMarketDocumentCommand).Assembly);
    opts.Discovery.IncludeAssembly(typeof(InfrastructureServiceExtensions).Assembly);

    opts.CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<ReadDbContext>();
});

builder.Services.Configure<MarketDocumentImportOptions>(
    builder.Configuration.GetSection(MarketDocumentImportOptions.SectionName));

var cronSchedule = builder.Configuration["MarketDocumentImport:CronSchedule"] ?? "0 0 2 * * ?";

builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<MarketDocumentImportJob>(
        trigger => trigger
            .WithIdentity("MarketDocumentImportTrigger")
            .WithCronSchedule(cronSchedule),
        job => job.WithIdentity("MarketDocumentImportJob"));
});

builder.Services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
