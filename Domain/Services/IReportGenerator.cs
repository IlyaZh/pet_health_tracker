using System.Text;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IReportGenerator
{
    Task<ReportResult> GenerateAsync(ReportContext context, CancellationToken ct);
}