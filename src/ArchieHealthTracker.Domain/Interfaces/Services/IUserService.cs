using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Interfaces.Services;

public interface IUserService
{
    Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username,
        CancellationToken ct);
}
