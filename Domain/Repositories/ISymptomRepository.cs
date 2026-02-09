using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface ISymptomRepository
{
    Task AddSymptom(SymptomEntry entry, CancellationToken ct);
}