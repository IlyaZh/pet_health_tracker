using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface IWeightRepository
{
    Task AddOrUpdateWeight(WeightEntry entry, CancellationToken ct);

    public Task<List<WeightEntry>> GetFilteredAsync(QueryParams parameters, CancellationToken ct);
}