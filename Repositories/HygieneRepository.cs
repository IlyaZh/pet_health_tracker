using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public class HygieneRepository : IHygieneRepository
{
    private readonly AppDbContext _dbContext;

    public HygieneRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddEvent(HygieneEntry entry, CancellationToken ct)
    {
        await _dbContext.AddAsync(entry, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}