using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Infrastructure.Data;
using ArchieHealthTracker.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Infrastructure.Repositories;

public class SymptomRepository(AppDbContext dbContext) : ISymptomRepository
{
    public async Task AddSymptomAsync(SymptomEntry entry, CancellationToken ct)
    {
        await dbContext.Symptoms.AddAsync(entry, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<SymptomEntry>> GetFilteredAsync(
        SymptomType? symptomType,
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = dbContext.Symptoms.AsQueryable();

        if (symptomType.HasValue) query = query.Where(x => x.Symptom == symptomType);

        return await query.ApplyBaseParams(parameters).ToListAsync(ct);
    }
}