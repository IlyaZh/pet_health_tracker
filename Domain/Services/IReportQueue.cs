using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface IReportQueue
{
    ValueTask EnqueueReportAsync(ReportQueueItem item);
    ValueTask<ReportQueueItem> DequeueReportAsync(CancellationToken ct);
}