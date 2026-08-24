using Ardalis.Result;
using Microsoft.Extensions.Options;
using Quartz;
using UseCases.Market.MarketDocuments.Import;
using Wolverine;

namespace Worker.Jobs;

public class MarketDocumentImportJob(
    IMessageBus bus,
    IOptions<MarketDocumentImportOptions> options,
    ILogger<MarketDocumentImportJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var opts = options.Value;

        logger.LogInformation("Starting MarketDocument import from {RemotePath}", opts.RemoteFilePath);

        var result = await bus.InvokeAsync<Result<long>>(
            new ImportMarketDocumentCommand(opts.RemoteFilePath, opts.CompanyId),
            context.CancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("MarketDocument import failed: {Errors}", string.Join(", ", result.Errors));
        }
    }
}
