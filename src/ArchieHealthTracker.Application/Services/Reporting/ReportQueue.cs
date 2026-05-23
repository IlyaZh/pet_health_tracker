using System.Threading.Channels;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Application.Services.Reporting;

public class ReportQueue : IReportQueue
{
    private const int BufferSize = 10;
    private readonly ILogger<ReportQueue> _logger;
    private readonly Channel<ReportQueueItem> _queue;

    public ReportQueue(
        ILogger<ReportQueue> logger
    )
    {
        _logger = logger;
        var options = new BoundedChannelOptions(BufferSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        _queue = Channel.CreateBounded<ReportQueueItem>(options);
    }


    public async ValueTask EnqueueReportAsync(ReportQueueItem item)
    {
        _logger.LogInformation($"[ReportQueue] Enqueueing {item}");
        await _queue.Writer.WriteAsync(item);
    }

    public async ValueTask<ReportQueueItem> DequeueReportAsync(CancellationToken ct)
    {
        _logger.LogInformation($"[ReportQueue] Dequeueing {_queue.Reader.Count}");
        return await _queue.Reader.ReadAsync(ct);
    }
}