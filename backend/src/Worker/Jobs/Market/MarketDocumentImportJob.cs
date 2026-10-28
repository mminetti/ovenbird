using Quartz;
using UseCases.Common;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using Wolverine;
using Core.Common.Constants;

namespace Worker.Jobs.Market;

public class MarketDocumentImportJob(
    IMessageBus bus,
    ProcessRunTracker processRunTracker,
    ILogger<MarketDocumentImportJob> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MarketDocument import");

        try
        {
            var run = await processRunTracker.StartAsync(Constants.ProcessTypes.MarketDocumentImport, cancellationToken);

            var result = await bus.InvokeAsync<ImportMarketDocumentResult>(
                new MarketDocumentImportCommand(), cancellationToken);

            var succeeded = result.Errors.Count == 0;

            await processRunTracker.CompleteAsync(run.Id, succeeded, result.Errors, cancellationToken);

            if (!succeeded)
            {
                logger.LogError("MarketDocument import failed. ProcessRunId={ProcessRunId}, Errors={Errors}",
                    run.Id, string.Join(", ", result.Errors.Select(v => v.ErrorMessage)));
            }
            else
            {
                logger.LogInformation(
                    "Imported {ImportedCount} MarketDocument(s). ProcessRunId={ProcessRunId}",
                    result.Imported.Count, run.Id);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to import {Message}", ex.Message);
        }
    }
}
