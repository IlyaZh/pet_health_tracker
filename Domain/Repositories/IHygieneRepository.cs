using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IHygieneRepository
{
    Task AddEventAsync(HygieneEntry entry, CancellationToken ct);
    Task<List<HygieneEntry>> GetFilteredAsync(HygieneEventType? eventType, QueryParams parameters, CancellationToken ct);
}