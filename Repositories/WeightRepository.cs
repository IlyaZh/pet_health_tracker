using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class WeightRepository : IWeightRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public WeightRepository(AppDbContext context, ILogger<WeightRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task UpsertWeight(WeightEntry entry, CancellationToken ct)
    {
        var existing = await _context.Weights.FirstOrDefaultAsync(e => e.Date == entry.Date, ct);
        if (existing != null)
        {
            existing.Weight = entry.Weight;
            existing.UserId = entry.UserId;
            existing.UpdatedAt = DateTime.UtcNow;

            _logger.LogDebug("Обновлена запись веса за {Date}", existing.Date);
        }
        else
        {
            await _context.Weights.AddAsync(entry, ct);
        }

        await _context.SaveChangesAsync(ct);
    }
}