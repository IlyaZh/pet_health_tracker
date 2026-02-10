using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public class MedicalEventRepository : IMedicalEventRepository
{
    private readonly AppDbContext _context;

    public MedicalEventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddEvent(MedicalEventEntry entry, CancellationToken ct)
    {
        await _context.MedicalEvents.AddAsync(entry, ct);
        await _context.SaveChangesAsync(ct);
    }
}