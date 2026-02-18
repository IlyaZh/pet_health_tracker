using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Domain.Generators;

public interface IReportGenerator<TOutput>
{
    Task<TOutput> GenerateAsync(ReportContext context);
}