using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Domain.Entities;

public record ReportQueueItem(
    ReportRequest Request,
    ReportFormat Format,
    long ChatId
);
