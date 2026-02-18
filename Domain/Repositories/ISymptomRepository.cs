using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface ISymptomRepository
{
    Task AddSymptomAsync(SymptomEntry entry, CancellationToken ct);

    public Task<List<SymptomEntry>> GetFilteredAsync(
        SymptomType? symptomType,
        QueryParams parameters,
        CancellationToken ct
    );
}