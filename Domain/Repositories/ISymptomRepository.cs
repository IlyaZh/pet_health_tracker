using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface ISymptomRepository
{
    Task AddSymptomAsync(SymptomEntry entry, CancellationToken ct);

    public Task<List<SymptomEntry>> GetFilteredAsync(
        SymptomType? symptomType,
        QueryParams parameters,
        CancellationToken ct
    );
}