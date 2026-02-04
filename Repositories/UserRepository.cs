using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<BotUser?> GetByTelegramIdAsync(long telegramId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
    }

    public async Task AddAsync(BotUser user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}