using Ardalis.Result;
using Quartz;
using UseCases.Market.MarketDocuments.Export;
using Wolverine;

namespace Worker.Jobs.Market;

public class ExportMarketDocumentItemJob(
    IMessageBus bus,
    ILogger<ExportMarketDocumentItemJob> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MarketDocumentItem export");

        var result = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
            new ExportMarketDocumentItemCommand(), cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("MarketDocumentItem export failed: {Errors}", string.Join(", ", result.Errors));
        }
        else
        {
            logger.LogInformation("Exported {Count} MarketDocumentItem(s)", result.Value.Count);
        }
    }
}
