using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IMedicalEventRepository
{
    public Task AddEvent(MedicalEventEntry entry, CancellationToken ct);
}