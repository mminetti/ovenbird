using Infrastructure;
using Infrastructure.Data;
using Quartz;
using UseCases.Market.MarketDocuments.Import.Strategies;
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
    opts.CodeGeneration.AlwaysUseServiceLocationFor<MarketImportStrategyResolver>();
});

var cronSchedule = builder.Configuration["MarketDocumentImport:CronSchedule"] ?? "0 0 2 * * ?";

builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<MarketDocumentImportJob>(
        trigger => trigger
            .WithIdentity("MarketDocumentImportTrigger")
            .WithCronSchedule(cronSchedule),
        job => job.WithIdentity("MarketDocumentImportJob"));

    //var queueKey = JobKey.Create(nameof(MarketDocumentImportJob));

    //q.AddJob<MarketDocumentImportJob>(jobBuilder => jobBuilder.WithIdentity(queueKey))
    //    .AddTrigger(trigger =>
    //        trigger
    //        .ForJob(queueKey)
    //        .WithSimpleSchedule(x => x.WithIntervalInHours(1))
    //    .StartNow());
});

builder.Services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
