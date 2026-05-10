using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface IHygieneRepository
{
    Task AddEventAsync(HygieneEntry entry, CancellationToken ct);
    Task<List<HygieneEntry>> GetFilteredAsync(HygieneEventType? eventType, QueryParams parameters, CancellationToken ct);
}