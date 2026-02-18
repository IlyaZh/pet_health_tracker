using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IMedicalEventRepository
{
    public Task AddEventAsync(MedicalEventEntry entry, CancellationToken ct);

    public Task<List<MedicalEventEntry>> GetFilteredAsync(
        MedicalEventType? eventType,
        QueryParams parameters,
        CancellationToken ct
    );
}