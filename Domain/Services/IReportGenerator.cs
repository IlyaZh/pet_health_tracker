using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface IReportGenerator
{
    Task<ReportResult> GenerateAsync(ReportContext context, CancellationToken ct);
}