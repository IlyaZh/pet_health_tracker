using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<BotUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId, ct);
    }

    public async Task AddAsync(BotUser user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
    }

}