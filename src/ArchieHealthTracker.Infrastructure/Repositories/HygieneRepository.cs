using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Infrastructure.Data;
using ArchieHealthTracker.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Infrastructure.Repositories;

public class HygieneRepository(
    AppDbContext dbContext,
    ILogger<HygieneRepository> logger)
    : IHygieneRepository
{
    public async Task AddEventAsync(HygieneEntry entry, CancellationToken ct)
    {
        var existing = await dbContext.Hygiene
            .FirstOrDefaultAsync(e => e.Date == entry.Date && e.Event == entry.Event, ct);
        if (existing != null)
        {
            logger.LogInformation("Entry has already exist, skip");
            return;
        }

        await dbContext.AddAsync(entry, ct);

        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<HygieneEntry>> GetFilteredAsync(
        HygieneEventType? eventType,
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = dbContext.Hygiene.AsQueryable();
        if (eventType.HasValue) query = query.Where(x => x.Event == eventType.Value);

        return await query.ApplyBaseParams(parameters).ToListAsync(ct);
    }
}
