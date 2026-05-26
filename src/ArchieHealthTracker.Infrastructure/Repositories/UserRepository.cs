using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<BotUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId, ct);
    }

    public async Task AddAsync(BotUser user, CancellationToken ct)
    {
        await dbContext.Users.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}
