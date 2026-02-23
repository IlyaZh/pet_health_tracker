using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Domain.Entities;

public record ReportResult(
    byte[] Content, 
    string FileName, 
    ReportFormat Format = ReportFormat.Telegram
);