using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface IUserRepository
{
    Task<BotUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);
    Task AddAsync(BotUser user, CancellationToken ct);
}