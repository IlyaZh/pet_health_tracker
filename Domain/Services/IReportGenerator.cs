using System.Text;
using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IReportGenerator<TOutput>
{
    Task<TOutput> GenerateAsync(ReportContext context, CancellationToken ct);
}