using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Repositories;

public class SymptomRepository : ISymptomRepository
{
    private readonly AppDbContext _context; 
    
    public SymptomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddSymptom(SymptomEntry entry, CancellationToken ct)
    {
        await _context.Symptoms.AddAsync(entry, ct);
        await _context.SaveChangesAsync(ct);
    }
}