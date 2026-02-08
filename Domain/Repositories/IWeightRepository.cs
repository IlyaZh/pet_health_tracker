using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IWeightRepository
{
    Task UpsertWeight(WeightEntry entry, CancellationToken ct);
}