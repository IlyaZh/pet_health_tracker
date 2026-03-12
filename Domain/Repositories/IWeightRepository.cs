using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IWeightRepository
{
    Task AddOrUpdateWeight(WeightEntry entry, CancellationToken ct);

    public Task<List<WeightEntry>> GetFilteredAsync(QueryParams parameters, CancellationToken ct);
}