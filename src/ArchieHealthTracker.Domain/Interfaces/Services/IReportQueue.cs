using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Interfaces.Services;

public interface IReportQueue
{
    ValueTask EnqueueReportAsync(ReportQueueItem item);
    ValueTask<ReportQueueItem> DequeueReportAsync(CancellationToken ct);
}
