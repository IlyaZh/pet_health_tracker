using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface IUserService
{
    Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username,
        CancellationToken ct);
}