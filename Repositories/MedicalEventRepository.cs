using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Repositories;

public class MedicalEventRepository : IMedicalEventRepository
{
    private readonly AppDbContext _dbContext;

    public MedicalEventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddEventAsync(MedicalEventEntry entry, CancellationToken ct)
    {
        await _dbContext.MedicalEvents.AddAsync(entry, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<MedicalEventEntry>> GetFilteredAsync(
        MedicalEventType? eventType,
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = _dbContext.MedicalEvents.AsQueryable();

        if (eventType.HasValue)
        {
            query = query.Where(x => x.Type == eventType.Value);
        }

        return await query.ApplyBaseParams(parameters).ToListAsync(ct);
    }
}