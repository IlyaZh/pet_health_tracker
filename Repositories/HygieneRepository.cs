using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Repositories;

public class HygieneRepository : IHygieneRepository
{
    private readonly AppDbContext _dbContext;

    public HygieneRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddEventAsync(HygieneEntry entry, CancellationToken ct)
    {
        await _dbContext.AddAsync(entry, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<HygieneEntry>> GetFilteredAsync(
        HygieneEventType? eventType,
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = _dbContext.Hygiene.AsQueryable();
        if (eventType.HasValue)
        {
            query = query.Where(x => x.Event == eventType.Value);
        }

        return await query.ApplyBaseParams(parameters).ToListAsync(ct);

    }
}