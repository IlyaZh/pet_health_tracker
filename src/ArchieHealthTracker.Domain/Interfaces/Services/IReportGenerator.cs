using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Interfaces.Services;

public interface IReportGenerator
{
    ReportResult Generate(ReportContext context, CancellationToken ct);
}