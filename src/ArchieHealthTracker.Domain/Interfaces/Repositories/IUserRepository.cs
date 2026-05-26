using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<BotUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);
    Task AddAsync(BotUser user, CancellationToken ct);
}
