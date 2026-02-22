using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IReportQueue
{
    ValueTask EnqueueReportAsync(ReportQueueItem item);
    ValueTask<ReportQueueItem> DequeueReportAsync(CancellationToken ct);
}