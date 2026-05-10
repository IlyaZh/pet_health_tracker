using ArchieHealthTracker.Data;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Repositories;
using ArchieHealthTracker.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class WeightRepository : IWeightRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger _logger;

    public WeightRepository(AppDbContext dbContext, ILogger<WeightRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddOrUpdateWeight(WeightEntry entry, CancellationToken ct)
    {
        var existing = await _dbContext.Weights.FirstOrDefaultAsync(e => e.Date == entry.Date, ct);
        if (existing != null)
        {
            existing.Weight = entry.Weight;
            existing.UserId = entry.UserId;
            existing.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Обновлена запись веса за {Date}", existing.Date);
        }
        else
        {
            await _dbContext.Weights.AddAsync(entry, ct);
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<WeightEntry>> GetFilteredAsync(
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = _dbContext.Weights.AsQueryable();
        return await query.ApplyBaseParams(parameters).ToListAsync(ct);
    }
}