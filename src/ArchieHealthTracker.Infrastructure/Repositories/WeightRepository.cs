using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Infrastructure.Data;
using ArchieHealthTracker.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Infrastructure.Repositories;

public class WeightRepository(AppDbContext dbContext, ILogger<WeightRepository> logger) : IWeightRepository
{
    private readonly ILogger _logger = logger;

    public async Task AddOrUpdateWeight(WeightEntry entry, CancellationToken ct)
    {
        var existing = await dbContext.Weights.FirstOrDefaultAsync(e => e.Date == entry.Date, ct);
        if (existing != null)
        {
            existing.Weight = entry.Weight;
            existing.UserId = entry.UserId;
            existing.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Обновлена запись веса за {Date}", existing.Date);
        }
        else
        {
            await dbContext.Weights.AddAsync(entry, ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<WeightEntry>> GetFilteredAsync(
        QueryParams parameters,
        CancellationToken ct
    )
    {
        var query = dbContext.Weights.AsQueryable();
        return await query.ApplyBaseParams(parameters).ToListAsync(ct);
    }
}
