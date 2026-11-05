using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Quartz;
using UseCases.Common;
using UseCases.Market.MarketDocuments.Import;
using Wolverine;

namespace Worker.Jobs.Market;

public class ImportMarketDocumentJob(
    IMessageBus bus,
    ProcessRunTracker processRunTracker,
    ILogger<ImportMarketDocumentJob> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MarketDocument import");

        try
        {
            var run = await processRunTracker.StartAsync(Constants.ProcessTypes.MarketDocumentImport, cancellationToken);

            IEnumerable<ValidationError> errors;

            try
            {
                var result = await bus.InvokeAsync<Result>(new MarketDocumentImportCommand(), cancellationToken);

                errors = result.ValidationErrors;
            }
            catch (Exception ex)
            {
                errors = [new ValidationError(nameof(MarketDocument), ex.Message)];
            }

            await processRunTracker.CompleteAsync(run.Id, errors, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to import {Message}", ex.Message);
        }
    }
}
