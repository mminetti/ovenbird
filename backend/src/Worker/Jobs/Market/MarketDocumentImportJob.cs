using Ardalis.Result;
using Quartz;
using UseCases.Common;
using UseCases.Market.MarketDocuments.Import;
using Wolverine;
using CoreConstants = Core.Common.Constants.Constants;

namespace Worker.Jobs.Market;

public class MarketDocumentImportJob(
    IMessageBus bus,
    ProcessRunTracker processRunTracker,
    ILogger<MarketDocumentImportJob> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MarketDocument import");

        var run = await processRunTracker.StartAsync(CoreConstants.ProcessTypes.MarketDocumentImport, cancellationToken);

        var result = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
            new ImportMarketDocumentCommand(), cancellationToken);

        await processRunTracker.CompleteAsync(run.Id, result.IsSuccess, result.ValidationErrors, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("MarketDocument import failed. ProcessRunId={ProcessRunId}, Errors={Errors}",
                run.Id, string.Join(", ", result.ValidationErrors.Select(v => v.ErrorMessage)));
        }
        else
        {
            logger.LogInformation("Imported {Count} MarketDocument(s). ProcessRunId={ProcessRunId}", result.Value.Count, run.Id);
        }
    }
}
