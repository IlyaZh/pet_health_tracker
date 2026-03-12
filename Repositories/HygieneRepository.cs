using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class HygieneRepository : IHygieneRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<HygieneRepository> _logger;

    public HygieneRepository(
        AppDbContext dbContext,
        ILogger<HygieneRepository> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddEventAsync(HygieneEntry entry, CancellationToken ct)
    {
        var existing = await _dbContext.Hygiene
            .FirstOrDefaultAsync(e => e.Date == entry.Date && e.Event == entry.Event, ct);
        if (existing != null)
        {
            _logger.LogInformation("Entry has already exist, skip");
            return;
        }
        else
        {
            await _dbContext.AddAsync(entry, ct);
        }

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