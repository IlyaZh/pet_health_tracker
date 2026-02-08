using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IUserService
{
    Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username,
        CancellationToken ct);
}