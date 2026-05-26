using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Infrastructure.Data;
using ArchieHealthTracker.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Infrastructure.Repositories;

public class MedicalEventRepository(
    AppDbContext dbContext,
    ILogger<MedicalEventRepository> logger)
    : IMedicalEventRepository
{
    private readonly ILogger<MedicalEventRepository> _logger = logger;

    public async Task AddEventAsync(MedicalEventEntry entry, CancellationToken ct)
    {
        await dbContext.MedicalEvents.AddAsync(entry, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<MedicalEventEntry>> GetFilteredAsync(
        MedicalEventType? eventType,
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = dbContext.MedicalEvents.AsQueryable();

        if (eventType.HasValue) query = query.Where(x => x.Type == eventType.Value);

        return await query.ApplyBaseParams(parameters).ToListAsync(ct);
    }
}
