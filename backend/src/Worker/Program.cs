using Core.MarketExchange;
using Infrastructure;
using Infrastructure.Data;
using Quartz;
using UseCases.Market.MarketDocuments.Export;
using UseCases.Market.MarketDocuments.Export.Interfaces;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using Wolverine;
using Worker.Jobs.Market;

var builder = Host.CreateApplicationBuilder(args);

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

builder.Services.AddInfrastructureServices(builder.Configuration, startupLogger);

builder.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(MarketDocument).Assembly);
    opts.Discovery.IncludeAssembly(typeof(MarketDocumentImportCommand).Assembly);
    opts.Discovery.IncludeAssembly(typeof(InfrastructureServiceExtensions).Assembly);

    opts.CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<ReadDbContext>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<MarketDocumentParserResolver>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<MarketDocumentGeneratorResolver>();
});

var cronSchedule = builder.Configuration["MarketDocumentImport:CronSchedule"] ?? "0 0 2 * * ?";
var exportItemCronSchedule = builder.Configuration["ExportMarketDocumentItem:CronSchedule"] ?? "0 30 1 * * ?";
var exportCronSchedule = builder.Configuration["ExportMarketDocument:CronSchedule"] ?? "0 0 3 * * ?";

builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<MarketDocumentImportJob>(
        trigger => trigger
            .WithIdentity("MarketDocumentImportTrigger")
            //.StartNow(),
            .WithCronSchedule(cronSchedule),
        job => job.WithIdentity("MarketDocumentImportJob"));

    q.ScheduleJob<ExportMarketDocumentItemJob>(
        trigger => trigger
            .WithIdentity("ExportMarketDocumentItemTrigger")
            .WithCronSchedule(exportItemCronSchedule),
        job => job.WithIdentity("ExportMarketDocumentItemJob"));

    q.ScheduleJob<ExportMarketDocumentJob>(
        trigger => trigger
            .WithIdentity("ExportMarketDocumentTrigger")
            .WithCronSchedule(exportCronSchedule),
        job => job.WithIdentity("ExportMarketDocumentJob"));
});

builder.Services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
