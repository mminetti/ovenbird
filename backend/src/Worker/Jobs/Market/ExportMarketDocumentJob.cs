using Ardalis.Result;
using Quartz;
using UseCases.Market.MarketDocuments.Export;
using Wolverine;

namespace Worker.Jobs.Market;

public class ExportMarketDocumentJob(
    IMessageBus bus,
    ILogger<ExportMarketDocumentJob> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MarketDocument export");

        var result = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
            new ExportMarketDocumentCommand(), cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("MarketDocument export failed: {Errors}", string.Join(", ", result.Errors));
        }
        else
        {
            logger.LogInformation("Exported {Count} MarketDocument(s)", result.Value.Count);
        }
    }
}
