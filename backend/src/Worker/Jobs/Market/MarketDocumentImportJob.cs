using Ardalis.Result;
using Microsoft.Extensions.Options;
using Quartz;
using UseCases.Market.MarketDocuments.Import;
using Wolverine;

namespace Worker.Jobs.Market;

public class MarketDocumentImportJob(
    IMessageBus bus,
    IOptions<MarketDocumentImportOptions> options,
    ILogger<MarketDocumentImportJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var opts = options.Value;

        logger.LogInformation("Starting MarketDocument import from {RemoteDirectory}", opts.RemoteDirectory);

        var result = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
            new ImportMarketDocumentCommand(), context.CancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("MarketDocument import failed: {Errors}", string.Join(", ", result.Errors));
        }
        else
        {
            logger.LogInformation("Imported {Count} MarketDocument(s)", result.Value.Count);
        }
    }
}
