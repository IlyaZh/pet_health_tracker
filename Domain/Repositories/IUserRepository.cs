using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IUserRepository
{
    Task<BotUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);
    Task AddAsync(BotUser user, CancellationToken ct);
}