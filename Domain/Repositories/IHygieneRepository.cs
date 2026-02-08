using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IHygieneRepository
{
    Task AddEvent(HygieneEntry entry, CancellationToken ct);
}