using Core.Market;
using Infrastructure;
using Infrastructure.Data;
using Quartz;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Processors;
using UseCases.Market.MarketDocuments.Import.Processors.Readers;
using UseCases.Market.MarketDocuments.Import.Strategies;
using Wolverine;
using Worker.Jobs.Market;

var builder = Host.CreateApplicationBuilder(args);

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

builder.Services.AddInfrastructureServices(builder.Configuration, startupLogger);

builder.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(MarketDocument).Assembly);
    opts.Discovery.IncludeAssembly(typeof(ImportMarketDocumentCommand).Assembly);
    opts.Discovery.IncludeAssembly(typeof(InfrastructureServiceExtensions).Assembly);

    opts.CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<ReadDbContext>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<MarketImportStrategyResolver>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<MarketDocumentItemProcessorResolver>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<MarketDocumentTransactionReaderResolver>();
});

var cronSchedule = builder.Configuration["MarketDocumentImport:CronSchedule"] ?? "0 0 2 * * ?";

builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<MarketDocumentImportJob>(
        trigger => trigger
            .WithIdentity("MarketDocumentImportTrigger")
            //.StartNow(),
            .WithCronSchedule(cronSchedule),
        job => job.WithIdentity("MarketDocumentImportJob"));
});

builder.Services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
