using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Repositories;

public class SymptomRepository : ISymptomRepository
{
    private readonly AppDbContext _dbContext; 
    
    public SymptomRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddSymptomAsync(SymptomEntry entry, CancellationToken ct)
    {
        await _dbContext.Symptoms.AddAsync(entry, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<SymptomEntry>> GetFilteredAsync(
        SymptomType? symptomType,
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = _dbContext.Symptoms.AsQueryable();

        if (symptomType.HasValue)
        {
            query = query.Where(x => x.Symptom == symptomType);
        }

        return await query.ApplyBaseParams(parameters).ToListAsync(ct);
    }
}