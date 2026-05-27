using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Interfaces.Repositories;

/// <summary>
/// Defines the repository operations for managing bot users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their Telegram ID.
    /// </summary>
    /// <param name="telegramId">The unique identifier from Telegram.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="BotUser"/> if found, otherwise null.</returns>
    Task<BotUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);

    /// <summary>
    /// Adds a new user to the system.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(BotUser user, CancellationToken ct);
}
