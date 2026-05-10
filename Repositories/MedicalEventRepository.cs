using ArchieHealthTracker.Data;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Repositories;
using ArchieHealthTracker.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class MedicalEventRepository : IMedicalEventRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<MedicalEventRepository> _logger;

    public MedicalEventRepository(
        AppDbContext dbContext,
        ILogger<MedicalEventRepository> logger
        )
    {
        _dbContext = dbContext;
        _logger = logger;
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