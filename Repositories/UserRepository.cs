using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<BotUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId, ct);

    }

    public async Task AddAsync(BotUser user, CancellationToken ct)
    {
        await _dbContext.Users.AddAsync(user, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}