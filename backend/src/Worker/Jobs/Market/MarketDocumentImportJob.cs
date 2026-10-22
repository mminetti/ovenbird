using Ardalis.Result;
using Quartz;
using UseCases.Market.MarketDocuments.Import;
using Wolverine;

namespace Worker.Jobs.Market;

public class MarketDocumentImportJob(
    IMessageBus bus,
    ILogger<MarketDocumentImportJob> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MarketDocument import");

        var result = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
            new ImportMarketDocumentCommand(), cancellationToken);

        if (!result.IsSuccess)
        {
            var messages = result.Errors
                .Concat(result.ValidationErrors.Select(v => $"{v.Identifier}: {v.ErrorMessage}"));

            logger.LogError("MarketDocument import failed: {Errors}", string.Join(", ", messages));
        }
        else
        {
            logger.LogInformation("Imported {Count} MarketDocument(s)", result.Value.Count);
        }
    }
}
