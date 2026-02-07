using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IUserRepository
{
    Task<BotUser?> GetByTelegramIdAsync(long telegramId);
    Task AddAsync(BotUser user);
    Task SaveChangesAsync();
}